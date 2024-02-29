using SiJwtAuth.Api.Dto.Command;
using SiJwtAuth.Data.Models;

namespace SiJwtAuth.Application.Services;

public interface IUserAuthService
{
    ValueTask<(User? user, bool ok)> AuthenticateAsync(string username, string password);

    ValueTask<Guid?> RegisterUserAsync(UserRegisterRequest userRegisterRequest);
}