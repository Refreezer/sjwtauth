using SiJwtAuth.Data.Models;

namespace SiJwtAuth.Application.Services;

public interface ITokenService
{
    Task<string> GenerateAccessTokenAsync(User user);

    ValueTask<string?> TryRefreshAsync(string tokenStr);
    bool IsAccessTokenActual(string tokenStr);
}