using System.IdentityModel.Tokens.Jwt;
using System.Text;
using Microsoft.IdentityModel.Tokens;
using SiJwtAuth.Application.Utils;
using SiJwtAuth.Data.Models;
using SiJwtAuth.Data.Repositories;

namespace SiJwtAuth.Application.Services.Impl;

public class TokenService(
    SecurityTokenHandler tokenHandler,
    ITokensFactory tokensFactory,
    ITokensRepository tokensRepository,
    IConfiguration conf)
    : ITokenService
{
    private const string JwtKeyConfigKey = "Jwt:Key";
    private const string JwtIssuerConfigKey = "Jwt:Issuer";

    private readonly TokenValidationParameters _tokenValidationParameters = new()
    {
        ValidateIssuer = true,
        ValidateAudience = false,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        ValidIssuer = conf[JwtIssuerConfigKey],
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(conf[JwtKeyConfigKey]!))
    };

    public bool IsAccessTokenActual(string tokenStr)
    {
        return TryReadToken(tokenStr, out var jwtToken) && IsActual(jwtToken!);
    }

    public async Task<string> GenerateAccessTokenAsync(User user)
    {
        var tokens = tokensFactory.Create(user.Id, user.Username, user.Email);
        await tokensRepository.SaveTokens(tokens);
        return tokens.AccessToken!;
    }

    public async ValueTask<string?> TryRefreshAsync(string tokenStr)
    {
        if (!TryReadToken(tokenStr, out var jwt, false))
        {
            return null;
        }

        if (IsActual(jwt!))
        {
            return tokenStr;
        }

        if (!Guid.TryParse(jwt!.Subject, out var userId))
        {
            return null;
        }

        var tokens = tokensFactory.CreateFromJwt(jwt, tokenStr, userId);
        return await tokensRepository.TryRefresh(tokens);
    }

    private bool TryReadToken(string tokenStr, out JwtSecurityToken? jwtToken, bool validate = true)
    {
        jwtToken = null;
        if (!tokenHandler.CanReadToken(tokenStr))
        {
            return false;
        }

        SecurityToken token;
        if (validate)
        {
            tokenHandler.ValidateToken(tokenStr, _tokenValidationParameters, out token);
        }
        else
        {
            token = tokenHandler.ReadToken(tokenStr);
        }

        if (token is not JwtSecurityToken jwtSecurityToken)
        {
            return false;
        }

        jwtToken = jwtSecurityToken;
        return true;
    }

    private static bool IsActual(SecurityToken jwtToken)
    {
        return jwtToken.ValidTo > DateTime.Now;
    }
}