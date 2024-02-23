
using SiJwtAuth.Dao.Models;

namespace SiJwtAuth.Services;

public interface IUserAuthService
{
    ValueTask<(User? user, bool ok)> AuthenticateAsync(string username, string password);
}