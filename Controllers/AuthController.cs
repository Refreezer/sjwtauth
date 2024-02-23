using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SiJwtAuth.Dto;
using SiJwtAuth.Dto.Response;
using SiJwtAuth.Services;

namespace SiJwtAuth.Controllers;

[Route("api/[controller]")]
[ApiController]
public class AuthController(
    IUserAuthService userAuthService,
    ITokenService tokenService
) : ControllerBase
{
    [AllowAnonymous]
    [HttpPost]
    public async Task<IActionResult> Login([FromBody] UserLoginRequest userLoginRequest) {
        var (user, ok) = await userAuthService.AuthenticateAsync(userLoginRequest.Username, userLoginRequest.Password);
        if (!ok) {
            return NotFound();
        }

        var token = tokenService.GenerateAccessToken(user!);
        return Ok(new UserLoginResponse(token, Guid.NewGuid().ToString("N")));
    }
}