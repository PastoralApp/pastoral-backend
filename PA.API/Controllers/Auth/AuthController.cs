using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PA.Application.DTOs;
using PA.Application.Interfaces.Repositories;
using PA.Application.Interfaces.Services;
using PA.Infrastructure.Auth;
using PA.Infrastructure.Data.Context;
using PA.Domain.ValueObjects;
using PA.Domain.Entities;
using System.Security.Claims;
using System.Security.Cryptography;

namespace PA.API.Controllers.Auth;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly IUserRepository _userRepository;
    private readonly JwtTokenService _jwtTokenService;
    private readonly GoogleAuthService _googleAuthService;
    private readonly IEmailService _emailService;
    private readonly IEmailVerificationRepository _emailVerificationRepository;
    private readonly PastoralAppDbContext _context;

    public AuthController(
        IUserRepository userRepository, 
        JwtTokenService jwtTokenService,
        GoogleAuthService googleAuthService,
        IEmailService emailService,
        IEmailVerificationRepository emailVerificationRepository,
        PastoralAppDbContext context)
    {
        _userRepository = userRepository;
        _jwtTokenService = jwtTokenService;
        _googleAuthService = googleAuthService;
        _emailService = emailService;
        _emailVerificationRepository = emailVerificationRepository;
        _context = context;
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
                Role = user.Role.Name
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
                await _emailVerificationRepository.DeleteByEmailAsync(result.Email!);
                
                var code = GenerateVerificationCode();
                var verification = new EmailVerification(
                    result.Email!, 
                    code, 
                    DateTime.UtcNow.AddMinutes(15), 
                    "registration"
                );
                await _emailVerificationRepository.AddAsync(verification);

                try
                {
                    await _emailService.SendVerificationCodeAsync(result.Email!, result.Name ?? "Usuário", code);
                }
                catch (Exception ex)
                {
                    return StatusCode(500, new { message = $"Erro ao enviar email: {ex.Message}" });
                }

                return Ok(new
                {
                    requiresCompletion = true,
                    registrationToken = result.RegistrationToken,
                    prefill = new
                    {
                        email = result.Email,
                        name = result.Name,
                        photoUrl = result.PhotoUrl
                    },
                    message = "Código de verificação enviado para o email"
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
                    Role = user.Role.Name
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
        if (string.IsNullOrWhiteSpace(dto.VerificationCode))
            return BadRequest(new { message = "Código de verificação é obrigatório" });

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

        var verification = await _emailVerificationRepository.GetValidCodeAsync(
            email, 
            dto.VerificationCode, 
            "registration"
        );

        if (verification == null)
            return BadRequest(new { message = "Código inválido ou expirado" });

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
                    Role = existing.Role.Name
                }
            });
        }

        verification.MarkAsUsed();
        await _emailVerificationRepository.UpdateAsync(verification);

        var defaultRole = await _context.Roles.FirstOrDefaultAsync(r => r.Type == PA.Domain.Enums.RoleType.Usuario);
        if (defaultRole == null)
            return StatusCode(500, new { message = "Erro ao obter role padrão" });

        var name = string.IsNullOrWhiteSpace(dto.Name) ? (nameFromToken ?? email) : dto.Name.Trim();
        var passwordHash = BCrypt.Net.BCrypt.HashPassword(dto.Password);
        var user = new PA.Domain.Entities.User(name, new Email(email), passwordHash, defaultRole.Id);
        user.VerifyEmail();
        if (!string.IsNullOrWhiteSpace(photoUrl))
            user.UpdateProfile(name, null, photoUrl);

        user = await _userRepository.AddAsync(user);

        await _emailVerificationRepository.DeleteByEmailAsync(email);

        try
        {
            await _emailService.SendWelcomeEmailAsync(email, name);
        }
        catch { }

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
                Role = user.Role.Name
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
            {
                await _emailVerificationRepository.DeleteByEmailAsync(result.Email!);
                
                var verificationCode = GenerateVerificationCode();
                var verification = new EmailVerification(
                    result.Email!, 
                    verificationCode, 
                    DateTime.UtcNow.AddMinutes(15), 
                    "registration"
                );
                await _emailVerificationRepository.AddAsync(verification);

                try
                {
                    await _emailService.SendVerificationCodeAsync(result.Email!, result.Name ?? "Usuário", verificationCode);
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Erro ao enviar email: {ex.Message}");
                }

                return Redirect($"http://localhost:4200/auth/google-complete?registrationToken={Uri.EscapeDataString(result.RegistrationToken!)}");
            }

            return Redirect($"http://localhost:4200/auth/google-success?token={Uri.EscapeDataString(result.Token!)}");
        }
        catch (Exception ex)
        {
            return Redirect($"http://localhost:4200/auth/login?error={Uri.EscapeDataString(ex.Message)}");
        }
    }

    [HttpPost("register/send-code")]
    public async Task<IActionResult> SendRegistrationCode([FromBody] SendCodeDto dto)
    {
        if (string.IsNullOrWhiteSpace(dto.Email))
            return BadRequest(new { message = "Email é obrigatório" });

        if (string.IsNullOrWhiteSpace(dto.Name))
            return BadRequest(new { message = "Nome é obrigatório" });

        var existingUser = await _userRepository.GetByEmailAsync(dto.Email);
        if (existingUser != null)
            return BadRequest(new { message = "Email já cadastrado" });

        await _emailVerificationRepository.DeleteByEmailAsync(dto.Email);
        
        var code = GenerateVerificationCode();
        var verification = new EmailVerification(
            dto.Email, 
            code, 
            DateTime.UtcNow.AddMinutes(15), 
            "registration"
        );
        await _emailVerificationRepository.AddAsync(verification);

        try
        {
            await _emailService.SendVerificationCodeAsync(dto.Email, dto.Name, code);
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { message = $"Erro ao enviar email: {ex.Message}" });
        }

        return Ok(new { 
            message = "Código de verificação enviado para o email",
            email = dto.Email 
        });
    }

    [HttpPost("register")]
    public async Task<IActionResult> Register([FromBody] RegisterDto dto)
    {
        if (string.IsNullOrWhiteSpace(dto.Email))
            return BadRequest(new { message = "Email é obrigatório" });
        
        if (string.IsNullOrWhiteSpace(dto.Password) || dto.Password.Length < 6)
            return BadRequest(new { message = "Senha deve ter no mínimo 6 caracteres" });

        if (string.IsNullOrWhiteSpace(dto.Name))
            return BadRequest(new { message = "Nome é obrigatório" });

        if (string.IsNullOrWhiteSpace(dto.VerificationCode))
            return BadRequest(new { message = "Código de verificação é obrigatório" });

        var verification = await _emailVerificationRepository.GetValidCodeAsync(
            dto.Email, 
            dto.VerificationCode, 
            "registration"
        );

        if (verification == null)
            return BadRequest(new { message = "Código inválido ou expirado" });

        var existingUser = await _userRepository.GetByEmailAsync(dto.Email);
        if (existingUser != null)
            return BadRequest(new { message = "Email já cadastrado" });

        verification.MarkAsUsed();
        await _emailVerificationRepository.UpdateAsync(verification);

        var defaultRole = await _context.Roles.FirstOrDefaultAsync(r => r.Type == PA.Domain.Enums.RoleType.Usuario);
        if (defaultRole == null)
            return StatusCode(500, new { message = "Erro ao obter role padrão" });

        var passwordHash = BCrypt.Net.BCrypt.HashPassword(dto.Password);
        var user = new PA.Domain.Entities.User(dto.Name, new Email(dto.Email), passwordHash, defaultRole.Id);
        user.VerifyEmail();

        user = await _userRepository.AddAsync(user);

        await _emailVerificationRepository.DeleteByEmailAsync(dto.Email);

        try
        {
            await _emailService.SendWelcomeEmailAsync(dto.Email, dto.Name);
        }
        catch { }

        var token = _jwtTokenService.GenerateToken(user);

        return Ok(new
        {
            token,
            user = new
            {
                user.Id,
                user.Name,
                Email = user.Email.Value,
                Role = user.Role.Name
            }
        });
    }

    [HttpPost("resend-verification-code")]
    public async Task<IActionResult> ResendVerificationCode([FromBody] ResendCodeDto dto)
    {
        if (string.IsNullOrWhiteSpace(dto.Email))
            return BadRequest(new { message = "Email é obrigatório" });

        var latest = await _emailVerificationRepository.GetLatestByEmailAsync(dto.Email, "registration");
        if (latest != null && latest.CreatedAt > DateTime.UtcNow.AddMinutes(-2))
        {
            return BadRequest(new { 
                message = "Aguarde alguns minutos antes de solicitar um novo código" 
            });
        }

        await _emailVerificationRepository.DeleteByEmailAsync(dto.Email);
        
        var code = GenerateVerificationCode();
        var verification = new EmailVerification(
            dto.Email, 
            code, 
            DateTime.UtcNow.AddMinutes(15), 
            "registration"
        );
        await _emailVerificationRepository.AddAsync(verification);

        try
        {
            await _emailService.SendVerificationCodeAsync(dto.Email, "Usuário", code);
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { message = $"Erro ao enviar email: {ex.Message}" });
        }

        return Ok(new { message = "Novo código enviado" });
    }

    private static string GenerateVerificationCode()
    {
        return RandomNumberGenerator.GetInt32(100000, 999999).ToString();
    }
}

public class RegisterDto
{
    public string Name { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
    public string VerificationCode { get; set; } = string.Empty;
}

public class SendCodeDto
{
    public string Name { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
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
    public string VerificationCode { get; set; } = string.Empty;
}

public class VerifyEmailDto
{
    public string Email { get; set; } = string.Empty;
    public string Code { get; set; } = string.Empty;
}

public class CompleteRegistrationDto
{
    public string Name { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
}

public class ResendCodeDto
{
    public string Email { get; set; } = string.Empty;
}
