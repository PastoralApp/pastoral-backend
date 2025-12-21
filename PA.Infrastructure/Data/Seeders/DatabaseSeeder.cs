using PA.Domain.Entities;
using PA.Domain.Enums;
using PA.Domain.ValueObjects;
using PA.Infrastructure.Data.Context;

namespace PA.Infrastructure.Data.Seeders;

/// <summary>
/// Classe responsável por popular o banco de dados com dados iniciais
/// </summary>
public static class DatabaseSeeder
{
    /// <summary>
    /// Executa o seed do banco de dados
    /// </summary>
    public static async Task SeedAsync(PastoralAppDbContext context)
    {
        // Verifica se já existem roles
        if (context.Roles.Any())
            return; // Já foi feito o seed

        Console.WriteLine("🌱 Iniciando seed do banco de dados...");

        // Criar Roles
        var roles = new[]
        {
            new Role(
                name: "Usuário",
                type: RoleType.Usuario,
                description: "Usuário comum da plataforma"
            ),
            new Role(
                name: "Coordenador de Grupo",
                type: RoleType.CoordenadorGrupo,
                description: "Coordenador de um grupo pastoral"
            ),
            new Role(
                name: "Coordenador Geral",
                type: RoleType.CoordenadorGeral,
                description: "Coordenador geral da pastoral"
            ),
            new Role(
                name: "Administrador",
                type: RoleType.Admin,
                description: "Administrador do sistema"
            )
        };

        await context.Roles.AddRangeAsync(roles);
        await context.SaveChangesAsync();
        Console.WriteLine("✅ Roles criadas");

        // Criar Pastorais
        var pastoralProfetaElias = new Pastoral(
            name: "Paróquia Profeta Elias",
            sigla: "PPE",
            tipoPastoral: TipoPastoral.PA,
            type: PastoralType.PA,
            theme: new ColorTheme("#8B4789", "#E0BBE4"),
            description: "Pastoral Adolescente da Paróquia Profeta Elias"
        );

        var pastoralPansa = new Pastoral(
            name: "Paróquia Nossa Senhora Aparecida",
            sigla: "PANSA",
            tipoPastoral: TipoPastoral.PA,
            type: PastoralType.PA,
            theme: new ColorTheme("#1E40AF", "#DBEAFE"),
            description: "Paróquia Nossa Senhora Aparecida"
        );

        await context.Pastorais.AddRangeAsync(pastoralProfetaElias, pastoralPansa);
        await context.SaveChangesAsync();
        Console.WriteLine("✅ Pastorais criadas");

        // Criar Grupos - Paróquia Profeta Elias
        var gruposProfetaElias = new[]
        {
            new Grupo("Adolescentes Renascendo na Fé", "AREF", "Grupo de adolescentes focado em renovação espiritual", pastoralProfetaElias.Id, new ColorTheme("#DC2626", "#FEE2E2")),
            new Grupo("Adolescentes Unidos em Cristo", "AUC", "União de adolescentes na fé católica", pastoralProfetaElias.Id, new ColorTheme("#2563EB", "#DBEAFE")),
            new Grupo("Adolescentes Gerando Amor Pelo Espírito Santo", "AGAPES", "Amor e comunhão no Espírito Santo", pastoralProfetaElias.Id, new ColorTheme("#7C3AED", "#EDE9FE")),
            new Grupo("Adolescentes Preservando a Vida", "APAV", "Promoção da cultura da vida", pastoralProfetaElias.Id, new ColorTheme("#059669", "#D1FAE5")),
            new Grupo("Adolescentes Juntos no Amor de Deus", "AJAD", "Unidos no amor de Deus", pastoralProfetaElias.Id, new ColorTheme("#EA580C", "#FED7AA")),
            new Grupo("Adolescentes Unidos Confirmando o Amor de Cristo", "AUCAC", "Confirmação do amor de Cristo", pastoralProfetaElias.Id, new ColorTheme("#DB2777", "#FCE7F3"))
        };

        // Criar Grupos - PANSA
        var gruposPansa = new[]
        {
            new Grupo("Adolescentes Unidos Pelo Espírito Santo", "AUPES", "Unidos no Espírito Santo", pastoralPansa.Id, new ColorTheme("#0891B2", "#CFFAFE")),
            new Grupo("Adolescentes Preservando a Vida", "APAV", "Cultura da vida", pastoralPansa.Id, new ColorTheme("#059669", "#D1FAE5")),
            new Grupo("Jovens Adolescentes no Amor de Deus", "JAAV", "Amor divino entre jovens", pastoralPansa.Id, new ColorTheme("#9333EA", "#F3E8FF")),
            new Grupo("Adolescentes Lutando pela Fé Ardente e Salvação", "ALFAS", "Fé ardente e salvação", pastoralPansa.Id, new ColorTheme("#DC2626", "#FEE2E2")),
            new Grupo("Adolescentes Unidos em Cristo", "AUC", "Unidade em Cristo", pastoralPansa.Id, new ColorTheme("#2563EB", "#DBEAFE")),
            new Grupo("Adolescentes Caminhando Alegremente com Luz de Jesus", "ACALJ", "Caminhando com alegria na luz de Jesus", pastoralPansa.Id, new ColorTheme("#F59E0B", "#FEF3C7"))
        };

        await context.Grupos.AddRangeAsync(gruposProfetaElias);
        await context.Grupos.AddRangeAsync(gruposPansa);
        await context.SaveChangesAsync();
        Console.WriteLine("✅ Grupos criados");

        // Criar Igrejas
        var igrejaMatriz = new Igreja(
            nome: "Igreja Matriz",
            endereco: "Endereço da Igreja Matriz",
            telefone: "(11) 1234-5678"
        );

        await context.Igrejas.AddAsync(igrejaMatriz);
        await context.SaveChangesAsync();
        Console.WriteLine("✅ Igrejas criadas");

        // Criar Horários de Missa
        var horariosMissa = new[]
        {
            // Domingo
            new HorarioMissa(igrejaMatriz.Id, DayOfWeek.Sunday, new TimeSpan(8, 0, 0), "Pe. FREI VAGNER"),
            new HorarioMissa(igrejaMatriz.Id, DayOfWeek.Sunday, new TimeSpan(10, 30, 0), "Pe. FREI VAGNER"),
            new HorarioMissa(igrejaMatriz.Id, DayOfWeek.Sunday, new TimeSpan(19, 30, 0), "Pe. FREI VAGNER"),
            // Sábado
            new HorarioMissa(igrejaMatriz.Id, DayOfWeek.Saturday, new TimeSpan(19, 30, 0), "Pe. FREI VAGNER"),
            // Dias de semana
            new HorarioMissa(igrejaMatriz.Id, DayOfWeek.Monday, new TimeSpan(19, 0, 0)),
            new HorarioMissa(igrejaMatriz.Id, DayOfWeek.Tuesday, new TimeSpan(19, 0, 0)),
            new HorarioMissa(igrejaMatriz.Id, DayOfWeek.Wednesday, new TimeSpan(19, 0, 0)),
            new HorarioMissa(igrejaMatriz.Id, DayOfWeek.Thursday, new TimeSpan(19, 0, 0)),
            new HorarioMissa(igrejaMatriz.Id, DayOfWeek.Friday, new TimeSpan(19, 0, 0))
        };

        await context.HorariosMissas.AddRangeAsync(horariosMissa);
        await context.SaveChangesAsync();
        Console.WriteLine("✅ Horários de Missa criados");

        // Buscar role de Admin
        var adminRole = roles.First(r => r.Type == RoleType.Admin);

        // Criar usuário Admin
        var adminEmail = new Email("admin@admin.com");
        var adminPasswordHash = BCrypt.Net.BCrypt.HashPassword("admin@admin");

        var adminUser = new User(
            name: "Administrador",
            email: adminEmail,
            passwordHash: adminPasswordHash,
            roleId: adminRole.Id
        );

        await context.Users.AddAsync(adminUser);
        await context.SaveChangesAsync();

        Console.WriteLine("✅ Usuário admin criado");
        Console.WriteLine($"   Email: admin@admin.com");
        Console.WriteLine($"   Senha: admin@admin");
        Console.WriteLine("🎉 Seed concluído!");
    }
}
