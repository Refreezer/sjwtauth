using Microsoft.EntityFrameworkCore;
using SiJwtAuth.Application.Utils;
using SiJwtAuth.Data.Models;

namespace SiJwtAuth.Data.Repositories.impl;

public class TokensRepository(
    IDbContextFactory<AuthContext> dbContextFactory,
    ITokensFactory tokensFactory
) : ITokensRepository
{
    public async Task<Tokens?> GetToken(Guid id)
    {
        await using var dbContext = await dbContextFactory.CreateDbContextAsync();
        return await dbContext.Tokens.FirstOrDefaultAsync(it => it.Id == id);
    }

    public async ValueTask SaveTokens(Tokens tokens)
    {
        await using var dbContext = await dbContextFactory.CreateDbContextAsync();
        var tx = await dbContext.Database.BeginTransactionAsync();
        var existed = await dbContext.Tokens.FirstOrDefaultAsync(it => it.Id == tokens.Id);
        if (existed != null)
        {
            dbContext.Tokens.Entry(existed).CurrentValues.SetValues(tokens);
        }
        else
        {
            await dbContext.Tokens.AddAsync(tokens);
        }

        await dbContext.SaveChangesAsync();
        await tx.CommitAsync();
    }

    public async ValueTask<string?> TryRefresh(Tokens tokens)
    {
        await using var dbContext = await dbContextFactory.CreateDbContextAsync();
        await using var tx = await dbContext.Database.BeginTransactionAsync();
        var storedTokens = await dbContext.Tokens.FirstOrDefaultAsync(it => it.Id == tokens.Id);
        if (storedTokens == null || !storedTokens.Equals(tokens))
        {
            return null;
        }

        var newTokens = tokensFactory.Create(storedTokens.Id, storedTokens.Username!, storedTokens.Email!);
        dbContext.Tokens.Entry(storedTokens).CurrentValues.SetValues(newTokens);
        await dbContext.SaveChangesAsync();
        await tx.CommitAsync();
        return newTokens.AccessToken!;
    }
}