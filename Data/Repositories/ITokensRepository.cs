using SiJwtAuth.Data.Models;

namespace SiJwtAuth.Data.Repositories;

public interface ITokensRepository
{
    Task<Tokens?> GetToken(Guid id);
    ValueTask SaveTokens(Tokens tokens);
    ValueTask<string?> TryRefresh(Tokens tokens);
}