using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using SocialMediaApp.Api.Configuration;

namespace SocialMediaApp.Api.Services;

public class JwtTokenService(IOptions<JwtOptions> options)
{
    private readonly JwtOptions jwtOptions = options.Value;

    public string CreateAccessToken(int userId, int sessionId)
    {
        var claims = new[]
        {
            new Claim(ClaimTypes.NameIdentifier, userId.ToString()),
            new Claim("sid", sessionId.ToString())
        };

        var credentials = new SigningCredentials(
            new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(jwtOptions.SecretKey)
            ),
            SecurityAlgorithms.HmacSha256
        );

        var token = new JwtSecurityToken(
            issuer: jwtOptions.Issuer,
            audience: jwtOptions.Audience,
            claims: claims,
            expires: DateTime.UtcNow.AddMinutes(jwtOptions.AccessTokenLifetimeMinutes),
            signingCredentials: credentials
        );

        return new JwtSecurityTokenHandler().WriteToken(token);
    }
}