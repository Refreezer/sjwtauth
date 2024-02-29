using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.IdentityModel.Tokens;
using SiJwtAuth.Data.Models;

namespace SiJwtAuth.Application.Utils.Impl;

public class TokensFactory(
    IConfiguration conf,
    SecurityTokenHandler tokenHandler,
    ITimespanParser timespanParser) : ITokensFactory
{
    private const string RefreshTokenClaim = "refresh_token";
    private readonly TimeSpan _accessTokenLifetime = conf["token_lifetime"] != null
        ? timespanParser.ParseExact(conf["token_lifetime"]!)
        : TimeSpan.FromMinutes(5);

    public Tokens Create(Guid id, string username, string email)
    {
        var symmetricSecurityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(conf[Const.ConfKey.JwtKey]!));
        var refreshToken = Guid.NewGuid().ToString("N");
        var token = new JwtSecurityToken(
            conf[Const.ConfKey.JwtIssuer],
            conf[Const.ConfKey.JwtAudience],
            new[]
            {
                new Claim(JwtRegisteredClaimNames.Sub, id.ToString()),
                new Claim(JwtRegisteredClaimNames.UniqueName, username),
                new Claim(JwtRegisteredClaimNames.Email, email),
                new Claim(RefreshTokenClaim, refreshToken)
            },
            null,
            DateTime.Now + _accessTokenLifetime,
            new SigningCredentials(symmetricSecurityKey, SecurityAlgorithms.HmacSha256)
        );

        var accessToken = tokenHandler.WriteToken(token);
        return new Tokens
        {
            Id = id,
            AccessToken = accessToken,
            RefreshToken = refreshToken,
            Username = username,
            Email = email
        };
    }

    public Tokens CreateFromJwt(JwtSecurityToken jwt, string accessToken, Guid id)
    {
        var tokens = new Tokens
        {
            Id = id,
            Username = jwt.Claims.FirstOrDefault(it => it.Type == JwtRegisteredClaimNames.UniqueName)?.Value,
            AccessToken = accessToken,
            RefreshToken = jwt.Claims.FirstOrDefault(it => it.Type == RefreshTokenClaim)?.Value,
            Email = jwt.Claims.FirstOrDefault(it => it.Type == JwtRegisteredClaimNames.Email)?.Value
        };

        return tokens;
    }
}