using SiJwtAuth.Api.Dto.Command;
using SiJwtAuth.Data.Models;
using SiJwtAuth.Data.Repositories;

namespace SiJwtAuth.Application.Services.impl;

public class UserAuthService(IUsersRepository repository) : IUserAuthService
{
    public async ValueTask<(User? user, bool ok)> AuthenticateAsync(string username, string password)
    {
        var user = await repository.SelectUserByUsernameAsync(username);
        if (user == null)
        {
            return (null, false);
        }

        if (user.PasswordHash != HashPassword(password, user.PasswordSalt))
        {
            return (user, false);
        }

        return (user, true);
    }

    public async ValueTask<Guid?> RegisterUserAsync(UserRegisterRequest userRegisterRequest)
    {
        var salt = BCrypt.Net.BCrypt.GenerateSalt(4);
        var user = new User
        {
            Username = userRegisterRequest.Username,
            PasswordSalt = salt,
            PasswordHash = HashPassword(userRegisterRequest.Password, salt),
            Email = userRegisterRequest.Email
        };

        return await repository.InsertUserAsync(user);
    }

    private static string HashPassword(string password, string salt)
    {
        return BCrypt.Net.BCrypt.HashPassword(password, salt);
    }
}