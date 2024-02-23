using SiJwtAuth.Dao.Models;

namespace SiJwtAuth.Repositories;

public interface IUsersRepository
{
    ValueTask<User?> SelectUserByUsernameAsync(string username);
}