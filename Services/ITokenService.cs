using System.Security.Claims;
using SiJwtAuth.Dao.Models;

namespace SiJwtAuth.Services;

public interface ITokenService
{
    string GenerateAccessToken(User user);
}