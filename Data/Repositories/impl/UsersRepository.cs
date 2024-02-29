using Microsoft.EntityFrameworkCore;
using SiJwtAuth.Data.Models;

namespace SiJwtAuth.Data.Repositories.impl;

public class UsersRepository(
    IDbContextFactory<AuthContext> dbContextFactory,
    ILogger<UsersRepository> logger
) : IUsersRepository
{
    public async ValueTask<User?> SelectUserByUsernameAsync(string username)
    {
        await using var dbContext = await dbContextFactory.CreateDbContextAsync();
        var usernameLower = username.ToLower();
        return await dbContext.Users.FirstOrDefaultAsync(it => it.Username == usernameLower);
    }

    public async ValueTask<Guid?> InsertUserAsync(User user)
    {
        await using var dbContext = await dbContextFactory.CreateDbContextAsync();
        await using var tx = await dbContext.Database.BeginTransactionAsync();
        user.Username = user.Username.ToLower();
        if (await dbContext.Users.AnyAsync(it => it.Username == user.Username || it.Email == user.Email))
        {
            return null;
        }

        var userEntry = await dbContext.Users.AddAsync(user);
        await dbContext.SaveChangesAsync();
        await tx.CommitAsync();
        logger.LogTrace("Insert user {id}", user.Id);
        return userEntry.Entity.Id;
    }
}