using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using PA.Application.Interfaces.Repositories;
using PA.Application.Interfaces.Services;
using PA.Application.Services;
using PA.Infrastructure.Auth;
using PA.Infrastructure.Data.Context;
using PA.Infrastructure.Repositories;

namespace PA.Infrastructure;

/// <summary>
/// Configuração de injeção de dependências da camada Infrastructure
/// </summary>
public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        // Database
        services.AddDbContext<PastoralAppDbContext>(options =>
            options.UseNpgsql(
                configuration.GetConnectionString("DefaultConnection"),
                b => b.MigrationsAssembly(typeof(PastoralAppDbContext).Assembly.FullName)));

        // Repositories
        services.AddScoped<IUserRepository, UserRepository>();
        services.AddScoped<IPostRepository, PostRepository>();
        services.AddScoped<IEventoRepository, EventoRepository>();
        services.AddScoped<IIgrejaRepository, IgrejaRepository>();
        services.AddScoped<IHorarioMissaRepository, HorarioMissaRepository>();
        services.AddScoped<IGrupoRepository, GrupoRepository>();
        services.AddScoped<IPastoralRepository, PastoralRepository>();

        // Services
        services.AddScoped<IUserService, UserService>();
        services.AddScoped<IEventoService, EventoService>();
        services.AddScoped<PostService>();

        // Auth
        services.Configure<JwtSettings>(options =>
        {
            var section = configuration.GetSection("JwtSettings");
            options.Secret = section["Secret"] ?? "";
            options.Issuer = section["Issuer"] ?? "";
            options.Audience = section["Audience"] ?? "";
            options.ExpirationHours = int.Parse(section["ExpirationHours"] ?? "24");
        });
        services.Configure<GoogleAuthSettings>(options =>
        {
            var section = configuration.GetSection("GoogleAuth");
            options.ClientId = section["ClientId"] ?? "";
            options.ClientSecret = section["ClientSecret"] ?? "";
            var redirectUris = new List<string>();
            foreach (var item in section.GetSection("RedirectUris").GetChildren())
            {
                if (item.Value != null)
                    redirectUris.Add(item.Value);
            }
            options.RedirectUris = redirectUris.ToArray();
        });
        services.AddScoped<JwtTokenService>();
        services.AddHttpClient<GoogleAuthService>();

        return services;
    }
}
