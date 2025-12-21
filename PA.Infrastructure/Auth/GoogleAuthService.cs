using Google.Apis.Auth;
using Microsoft.Extensions.Options;
using PA.Application.Interfaces.Repositories;
using PA.Domain.Entities;
using PA.Domain.ValueObjects;
using System.Net.Http.Json;
using System.Text.Json.Serialization;

namespace PA.Infrastructure.Auth;

/// <summary>
/// Serviço de autenticação com Google OAuth
/// </summary>
public class GoogleAuthService
{
    private readonly GoogleAuthSettings _settings;
    private readonly IUserRepository _userRepository;
    private readonly JwtTokenService _jwtTokenService;
    private readonly HttpClient _httpClient;

    public GoogleAuthService(
        IOptions<GoogleAuthSettings> settings,
        IUserRepository userRepository,
        JwtTokenService jwtTokenService,
        HttpClient httpClient)
    {
        _settings = settings.Value;
        _userRepository = userRepository;
        _jwtTokenService = jwtTokenService;
        _httpClient = httpClient;
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

    /// <summary>
    /// Retorna o Client ID do Google para uso no frontend
    /// </summary>
    public string GetClientId()
    {
        return _settings.ClientId;
    }

    /// <summary>
    /// Troca o código de autorização por tokens e informações do usuário
    /// </summary>
    public async Task<(User user, string token)> ExchangeCodeForTokenAsync(string code, string redirectUri, Guid defaultRoleId)
    {
        // Troca código por tokens
        var tokenResponse = await ExchangeAuthorizationCodeAsync(code, redirectUri);
        
        if (tokenResponse?.IdToken == null)
            throw new UnauthorizedAccessException("Falha ao obter token do Google");

        // Processa o login usando o ID token
        return await ProcessGoogleLoginAsync(tokenResponse.IdToken, defaultRoleId);
    }

    /// <summary>
    /// Troca código de autorização por tokens de acesso
    /// </summary>
    private async Task<GoogleTokenResponse?> ExchangeAuthorizationCodeAsync(string code, string redirectUri)
    {
        var tokenEndpoint = "https://oauth2.googleapis.com/token";
        
        var requestData = new Dictionary<string, string>
        {
            { "code", code },
            { "client_id", _settings.ClientId },
            { "client_secret", _settings.ClientSecret },
            { "redirect_uri", redirectUri },
            { "grant_type", "authorization_code" }
        };

        var response = await _httpClient.PostAsync(tokenEndpoint, new FormUrlEncodedContent(requestData));
        
        if (!response.IsSuccessStatusCode)
            return null;

        return await response.Content.ReadFromJsonAsync<GoogleTokenResponse>();
    }
}

/// <summary>
/// Resposta do endpoint de token do Google
/// </summary>
internal class GoogleTokenResponse
{
    [JsonPropertyName("access_token")]
    public string AccessToken { get; set; } = string.Empty;

    [JsonPropertyName("id_token")]
    public string IdToken { get; set; } = string.Empty;

    [JsonPropertyName("expires_in")]
    public int ExpiresIn { get; set; }

    [JsonPropertyName("token_type")]
    public string TokenType { get; set; } = string.Empty;

    [JsonPropertyName("refresh_token")]
    public string? RefreshToken { get; set; }
}
