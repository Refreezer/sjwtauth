using SiJwtAuth.Data.Models;

namespace SiJwtAuth.Data.Repositories;

public interface IUsersRepository
{
    ValueTask<User?> SelectUserByUsernameAsync(string username);
    ValueTask<Guid?> InsertUserAsync(User user);
}