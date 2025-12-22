using Microsoft.AspNetCore.Mvc;
using PA.Application.DTOs;
using PA.Application.Interfaces.Repositories;
using PA.Infrastructure.Auth;
using PA.Domain.ValueObjects;
using System.Security.Claims;

namespace PA.API.Controllers.Auth;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly IUserRepository _userRepository;
    private readonly JwtTokenService _jwtTokenService;
    private readonly GoogleAuthService _googleAuthService;

    public AuthController(
        IUserRepository userRepository, 
        JwtTokenService jwtTokenService,
        GoogleAuthService googleAuthService)
    {
        _userRepository = userRepository;
        _jwtTokenService = jwtTokenService;
        _googleAuthService = googleAuthService;
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginDto dto)
    {
        var user = await _userRepository.GetByEmailAsync(dto.Email);
        
        if (user == null || !BCrypt.Net.BCrypt.Verify(dto.Password, user.PasswordHash))
            return Unauthorized(new { message = "Email ou senha inválidos" });

        if (!user.IsActive)
            return Unauthorized(new { message = "Usuário inativo" });

        var token = _jwtTokenService.GenerateToken(user);

        return Ok(new
        {
            token,
            user = new
            {
                user.Id,
                user.Name,
                Email = user.Email.Value,
                Role = user.Role.Type.ToString()
            }
        });
    }

    [HttpPost("google")]
    public async Task<IActionResult> GoogleLogin([FromBody] GoogleLoginDto dto)
    {
        try
        {
            var result = await _googleAuthService.ProcessGoogleLoginWithCompletionAsync(dto.IdToken);

            if (result.RequiresCompletion)
            {
                return Ok(new
                {
                    requiresCompletion = true,
                    registrationToken = result.RegistrationToken,
                    prefill = new
                    {
                        email = result.Email,
                        name = result.Name,
                        photoUrl = result.PhotoUrl
                    }
                });
            }

            var user = result.User!;
            return Ok(new
            {
                requiresCompletion = false,
                token = result.Token,
                user = new
                {
                    user.Id,
                    user.Name,
                    Email = user.Email.Value,
                    user.PhotoUrl,
                    Role = user.Role.Type.ToString()
                }
            });
        }
        catch (UnauthorizedAccessException ex)
        {
            return Unauthorized(new { message = ex.Message });
        }
    }

    [HttpPost("google/complete")]
    public async Task<IActionResult> CompleteGoogleRegistration([FromBody] GoogleCompleteRegistrationDto dto)
    {
        if (string.IsNullOrWhiteSpace(dto.RegistrationToken))
            return BadRequest(new { message = "registrationToken é obrigatório" });
        if (string.IsNullOrWhiteSpace(dto.Password) || dto.Password.Length < 6)
            return BadRequest(new { message = "Senha inválida (mínimo 6 caracteres)" });

        var principal = _jwtTokenService.ValidateToken(dto.RegistrationToken);
        if (principal == null)
            return Unauthorized(new { message = "registrationToken inválido" });

        var purpose = principal.FindFirst("purpose")?.Value;
        if (!string.Equals(purpose, "google_registration", StringComparison.Ordinal))
            return Unauthorized(new { message = "registrationToken inválido" });

        var email = principal.FindFirst(ClaimTypes.Email)?.Value;
        var nameFromToken = principal.FindFirst(ClaimTypes.Name)?.Value;
        var photoUrl = principal.FindFirst("photo_url")?.Value;

        if (string.IsNullOrWhiteSpace(email))
            return Unauthorized(new { message = "registrationToken inválido" });

        var existing = await _userRepository.GetByEmailAsync(email);
        if (existing != null)
        {
            if (!existing.IsActive)
                return Unauthorized(new { message = "Usuário inativo" });

            var existingToken = _jwtTokenService.GenerateToken(existing);
            return Ok(new
            {
                token = existingToken,
                user = new
                {
                    existing.Id,
                    existing.Name,
                    Email = existing.Email.Value,
                    existing.PhotoUrl,
                    Role = existing.Role.Type.ToString()
                }
            });
        }

        var defaultRoleId = Guid.Parse("00000000-0000-0000-0000-000000000001");
        var name = string.IsNullOrWhiteSpace(dto.Name) ? (nameFromToken ?? email) : dto.Name.Trim();
        var passwordHash = BCrypt.Net.BCrypt.HashPassword(dto.Password);
        var user = new PA.Domain.Entities.User(name, new Email(email), passwordHash, defaultRoleId);
        if (!string.IsNullOrWhiteSpace(photoUrl))
            user.UpdateProfile(name, null, photoUrl);

        user = await _userRepository.AddAsync(user);
        var token = _jwtTokenService.GenerateToken(user);

        return Ok(new
        {
            token,
            user = new
            {
                user.Id,
                user.Name,
                Email = user.Email.Value,
                user.PhotoUrl,
                Role = user.Role.Type.ToString()
            }
        });
    }

    [HttpGet("google-login")]
    public IActionResult GoogleLoginRedirect()
    {
        var clientId = _googleAuthService.GetClientId();
        var redirectUri = $"{Request.Scheme}://{Request.Host}/api/auth/google-callback";
        var scope = "openid profile email";
        var responseType = "code";
        
        var googleAuthUrl = $"https://accounts.google.com/o/oauth2/v2/auth?" +
            $"client_id={clientId}&" +
            $"redirect_uri={Uri.EscapeDataString(redirectUri)}&" +
            $"response_type={responseType}&" +
            $"scope={Uri.EscapeDataString(scope)}&" +
            $"access_type=offline&" +
            $"prompt=consent";

        return Redirect(googleAuthUrl);
    }

    [HttpGet("google-callback")]
    public async Task<IActionResult> GoogleCallback([FromQuery] string code, [FromQuery] string? error)
    {
        if (!string.IsNullOrEmpty(error))
            return Redirect($"http://localhost:4200/auth/login?error={Uri.EscapeDataString(error)}");

        if (string.IsNullOrEmpty(code))
            return Redirect("http://localhost:4200/auth/login?error=no_code");

        try
        {
            var redirectUri = $"{Request.Scheme}://{Request.Host}/api/auth/google-callback";

            var result = await _googleAuthService.ExchangeCodeForLoginWithCompletionAsync(code, redirectUri);

            if (result.RequiresCompletion)
                return Redirect($"http://localhost:4200/auth/google-complete?registrationToken={Uri.EscapeDataString(result.RegistrationToken!)}");

            return Redirect($"http://localhost:4200/auth/google-success?token={Uri.EscapeDataString(result.Token!)}");
        }
        catch (Exception ex)
        {
            return Redirect($"http://localhost:4200/auth/login?error={Uri.EscapeDataString(ex.Message)}");
        }
    }
}

public class LoginDto
{
    public string Email { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
}

public class GoogleLoginDto
{
    public string IdToken { get; set; } = string.Empty;
}

public class GoogleCompleteRegistrationDto
{
    public string RegistrationToken { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
}
