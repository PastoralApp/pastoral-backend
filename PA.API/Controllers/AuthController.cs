using Microsoft.AspNetCore.Mvc;
using PA.Application.DTOs;
using PA.Application.Interfaces.Repositories;
using PA.Infrastructure.Auth;

namespace PA.API.Controllers;

/// <summary>
/// Controller de autenticação
/// </summary>
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
            // Role padrão para novos usuários (Usuario comum)
            // TODO: Buscar RoleId do banco ou configurar
            var defaultRoleId = Guid.Parse("00000000-0000-0000-0000-000000000001"); 

            var (user, token) = await _googleAuthService.ProcessGoogleLoginAsync(dto.IdToken, defaultRoleId);

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
        catch (UnauthorizedAccessException ex)
        {
            return Unauthorized(new { message = ex.Message });
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
