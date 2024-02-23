using SiJwtAuth.Dao.Models;
using SiJwtAuth.Repositories;

namespace SiJwtAuth.Services.impl;

public class UserAuthService(IUsersRepository repository) : IUserAuthService
{
    private const string Salt = "8e0710ec7dc64ba2b518a084c791277a";
    public async ValueTask<(User? user, bool ok)> AuthenticateAsync(string username, string password)
    {
        var user = await repository.SelectUserByUsernameAsync(username);
        if (user == null)
        {
            return (null, false);
        }

        if (user.PasswordHash != BCrypt.Net.BCrypt.HashPassword(user.PasswordHash, Salt)) 
        {
            return (user, false);
        }
        
        return (user, true);
    }
}