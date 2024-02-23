using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.IdentityModel.Tokens;
using SiJwtAuth.Dao.Models;

namespace SiJwtAuth.Services.impl;

public class TokenService(
    IConfiguration conf,
    SecurityTokenHandler tokenHandler
) : ITokenService
{
    public string GenerateAccessToken(User user)
    {
        var symmetricSecurityKey =
            new SymmetricSecurityKey(Encoding.UTF8.GetBytes(conf[Const.ConfKey.JwtKey]!));
        var token = new JwtSecurityToken(
            conf[Const.ConfKey.JwtIssuer],
            conf[Const.ConfKey.JwtAudience],
            new[]
            {
                new Claim(JwtRegisteredClaimNames.Sub, user.Username),
                new Claim(JwtRegisteredClaimNames.Email, user.Email)
            },
            null,
            DateTime.Now.AddMinutes(5),
            new SigningCredentials(symmetricSecurityKey, SecurityAlgorithms.HmacSha256)
        );

        return tokenHandler.WriteToken(token);
    }
}