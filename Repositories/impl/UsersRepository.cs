using Microsoft.EntityFrameworkCore;
using SiJwtAuth.Controllers;
using SiJwtAuth.Dao;
using SiJwtAuth.Dao.Models;

namespace SiJwtAuth.Repositories.impl;

public class UsersRepository(AuthContext authContext) : IUsersRepository
{
    public async ValueTask<User?> SelectUserByUsernameAsync(string username)
        => await authContext.Users.FirstOrDefaultAsync(it => it.Username == username);
}