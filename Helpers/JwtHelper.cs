using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using LadyRuth.API.Entities;
using Microsoft.IdentityModel.Tokens;

namespace LadyRuth.API.Helpers;

public class JwtHelper(IConfiguration config)
{
    public (string Token, DateTime ExpiresAt) Generate(AdminUser user)
    {
        var jwtSection = config.GetSection("Jwt");
        var secret     = jwtSection["Secret"]!;
        var issuer     = jwtSection["Issuer"]!;
        var audience   = jwtSection["Audience"]!;
        var expiryHours = int.Parse(jwtSection["ExpiryHours"] ?? "8");

        var key         = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secret));
        var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
        var expiresAt   = DateTime.UtcNow.AddHours(expiryHours);

        var claims = new[]
        {
            new Claim(JwtRegisteredClaimNames.Sub, user.Id.ToString()),
            new Claim(JwtRegisteredClaimNames.Email, user.Email),
            new Claim(ClaimTypes.Role, user.Role),
            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
        };

        var token = new JwtSecurityToken(
            issuer:             issuer,
            audience:           audience,
            claims:             claims,
            expires:            expiresAt,
            signingCredentials: credentials);

        return (new JwtSecurityTokenHandler().WriteToken(token), expiresAt);
    }
}
