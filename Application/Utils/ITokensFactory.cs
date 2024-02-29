using System.IdentityModel.Tokens.Jwt;
using SiJwtAuth.Data.Models;

namespace SiJwtAuth.Application.Utils;

public interface ITokensFactory
{
    Tokens Create(Guid id, string username, string email);
    Tokens CreateFromJwt(JwtSecurityToken jwt, string accessToken, Guid id);
}