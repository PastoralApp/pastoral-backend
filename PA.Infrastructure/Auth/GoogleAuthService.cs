using Google.Apis.Auth;
using Microsoft.Extensions.Options;
using PA.Application.Interfaces.Repositories;
using PA.Domain.Entities;
using PA.Domain.ValueObjects;

namespace PA.Infrastructure.Auth;

/// <summary>
/// Serviço de autenticação com Google OAuth
/// </summary>
public class GoogleAuthService
{
    private readonly GoogleAuthSettings _settings;
    private readonly IUserRepository _userRepository;
    private readonly JwtTokenService _jwtTokenService;

    public GoogleAuthService(
        IOptions<GoogleAuthSettings> settings,
        IUserRepository userRepository,
        JwtTokenService jwtTokenService)
    {
        _settings = settings.Value;
        _userRepository = userRepository;
        _jwtTokenService = jwtTokenService;
    }

    /// <summary>
    /// Valida o token ID do Google e retorna informações do usuário
    /// </summary>
    public async Task<GoogleJsonWebSignature.Payload?> ValidateGoogleTokenAsync(string idToken)
    {
        try
        {
            var settings = new GoogleJsonWebSignature.ValidationSettings
            {
                Audience = new[] { _settings.ClientId }
            };

            var payload = await GoogleJsonWebSignature.ValidateAsync(idToken, settings);
            return payload;
        }
        catch
        {
            return null;
        }
    }

    /// <summary>
    /// Processa login com Google - cria usuário se não existir
    /// </summary>
    public async Task<(User user, string token)> ProcessGoogleLoginAsync(string idToken, Guid defaultRoleId)
    {
        var payload = await ValidateGoogleTokenAsync(idToken);
        
        if (payload == null)
            throw new UnauthorizedAccessException("Token do Google inválido");

        // Buscar usuário existente
        var user = await _userRepository.GetByEmailAsync(payload.Email);

        if (user == null)
        {
            // Criar novo usuário
            var email = new Email(payload.Email);
            var passwordHash = BCrypt.Net.BCrypt.HashPassword(Guid.NewGuid().ToString()); // Senha aleatória

            user = new User(
                name: payload.Name,
                email: email,
                passwordHash: passwordHash,
                roleId: defaultRoleId
            );

            if (!string.IsNullOrEmpty(payload.Picture))
                user.UpdateProfile(payload.Name, null, payload.Picture);

            user = await _userRepository.AddAsync(user);
        }

        if (!user.IsActive)
            throw new UnauthorizedAccessException("Usuário inativo");

        // Gerar token JWT
        var token = _jwtTokenService.GenerateToken(user);

        return (user, token);
    }
}
