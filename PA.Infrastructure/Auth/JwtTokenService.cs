using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using PA.Domain.Entities;
using System.Security.Claims;
using System.Text;
using Microsoft.IdentityModel.JsonWebTokens;

namespace PA.Infrastructure.Auth;

public class JwtTokenService
{
    private readonly JwtSettings _jwtSettings;

    public JwtTokenService(IOptions<JwtSettings> jwtSettings)
    {
        _jwtSettings = jwtSettings.Value;
    }

    public string GenerateToken(User user)
    {
        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtSettings.Secret));
        var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var descriptor = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(new[]
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new Claim(ClaimTypes.Name, user.Name),
                new Claim(ClaimTypes.Email, user.Email.Value),
                new Claim(ClaimTypes.Role, user.Role.Name),
                new Claim("RoleId", user.RoleId.ToString()),
                new Claim("RoleName", user.Role.Name)
            }),
            Expires = DateTime.UtcNow.AddHours(_jwtSettings.ExpirationHours),
            SigningCredentials = credentials,
            Issuer = _jwtSettings.Issuer,
            Audience = _jwtSettings.Audience
        };

        var tokenHandler = new JsonWebTokenHandler();
        return tokenHandler.CreateToken(descriptor);
    }

    public string GenerateGoogleRegistrationToken(string email, string name, string? photoUrl, string googleSubject)
    {
        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtSettings.Secret));
        var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var claims = new List<Claim>
        {
            new("purpose", "google_registration"),
            new Claim(ClaimTypes.Email, email),
            new Claim(ClaimTypes.Name, name),
            new("google_sub", googleSubject)
        };

        if (!string.IsNullOrWhiteSpace(photoUrl))
            claims.Add(new("photo_url", photoUrl));

        var descriptor = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(claims),
            Expires = DateTime.UtcNow.AddMinutes(15),
            SigningCredentials = credentials,
            Issuer = _jwtSettings.Issuer,
            Audience = _jwtSettings.Audience
        };

        var tokenHandler = new JsonWebTokenHandler();
        return tokenHandler.CreateToken(descriptor);
    }

    public ClaimsPrincipal? ValidateToken(string token)
    {
        var tokenHandler = new JsonWebTokenHandler();
        var key = Encoding.UTF8.GetBytes(_jwtSettings.Secret);

        try
        {
            var result = tokenHandler.ValidateTokenAsync(token, new TokenValidationParameters
            {
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(key),
                ValidateIssuer = true,
                ValidIssuer = _jwtSettings.Issuer,
                ValidateAudience = true,
                ValidAudience = _jwtSettings.Audience,
                ValidateLifetime = true,
                ClockSkew = TimeSpan.Zero
            }).Result;

            if (!result.IsValid)
                return null;

            return new ClaimsPrincipal(result.ClaimsIdentity);
        }
        catch
        {
            return null;
        }
    }
}
