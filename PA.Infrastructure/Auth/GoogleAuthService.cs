using Google.Apis.Auth;
using Microsoft.Extensions.Options;
using PA.Application.Interfaces.Repositories;
using PA.Domain.Entities;
using PA.Domain.ValueObjects;
using System.Net.Http.Json;
using System.Text.Json.Serialization;

namespace PA.Infrastructure.Auth;

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

    public async Task<(User user, string token)> ProcessGoogleLoginAsync(string idToken, Guid defaultRoleId)
    {
        var payload = await ValidateGoogleTokenAsync(idToken);
        
        if (payload == null)
            throw new UnauthorizedAccessException("Token do Google inválido");

        var user = await _userRepository.GetByEmailAsync(payload.Email);

        if (user == null)
        {
            var email = new Email(payload.Email);
            var passwordHash = BCrypt.Net.BCrypt.HashPassword(Guid.NewGuid().ToString()); 
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

        var token = _jwtTokenService.GenerateToken(user);

        return (user, token);
    }

    public async Task<GoogleLoginResult> ProcessGoogleLoginWithCompletionAsync(string idToken)
    {
        var payload = await ValidateGoogleTokenAsync(idToken);

        if (payload == null)
            throw new UnauthorizedAccessException("Token do Google inválido");

        var user = await _userRepository.GetByEmailAsync(payload.Email);
        if (user != null)
        {
            if (!user.IsActive)
                throw new UnauthorizedAccessException("Usuário inativo");

            var token = _jwtTokenService.GenerateToken(user);
            return GoogleLoginResult.Success(user, token);
        }

        // Usuário ainda não existe: exigir completar cadastro (ex: definir senha)
        var regToken = _jwtTokenService.GenerateGoogleRegistrationToken(
            email: payload.Email,
            name: payload.Name ?? payload.Email,
            photoUrl: payload.Picture,
            googleSubject: payload.Subject
        );

        return GoogleLoginResult.NeedsCompletion(
            registrationToken: regToken,
            email: payload.Email,
            name: payload.Name ?? payload.Email,
            photoUrl: payload.Picture
        );
    }

    public string GetClientId()
    {
        return _settings.ClientId;
    }

    public async Task<(User user, string token)> ExchangeCodeForTokenAsync(string code, string redirectUri, Guid defaultRoleId)
    {
        var tokenResponse = await ExchangeAuthorizationCodeAsync(code, redirectUri);
        
        if (tokenResponse?.IdToken == null)
            throw new UnauthorizedAccessException("Falha ao obter token do Google");

        return await ProcessGoogleLoginAsync(tokenResponse.IdToken, defaultRoleId);
    }

	public async Task<GoogleLoginResult> ExchangeCodeForLoginWithCompletionAsync(string code, string redirectUri)
	{
		var tokenResponse = await ExchangeAuthorizationCodeAsync(code, redirectUri);

		if (string.IsNullOrWhiteSpace(tokenResponse?.IdToken))
			throw new UnauthorizedAccessException("Falha ao obter token do Google");

		return await ProcessGoogleLoginWithCompletionAsync(tokenResponse.IdToken);
	}

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

public sealed record GoogleLoginResult(
    bool RequiresCompletion,
    User? User,
    string? Token,
    string? RegistrationToken,
    string? Email,
    string? Name,
    string? PhotoUrl)
{
    public static GoogleLoginResult Success(User user, string token) =>
        new(
            RequiresCompletion: false,
            User: user,
            Token: token,
            RegistrationToken: null,
            Email: null,
            Name: null,
            PhotoUrl: null
        );

    public static GoogleLoginResult NeedsCompletion(string registrationToken, string email, string name, string? photoUrl) =>
        new(
            RequiresCompletion: true,
            User: null,
            Token: null,
            RegistrationToken: registrationToken,
            Email: email,
            Name: name,
            PhotoUrl: photoUrl
        );
}

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
