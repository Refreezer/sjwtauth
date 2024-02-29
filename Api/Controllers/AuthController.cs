using System.Net;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SiJwtAuth.Api.Dto.Command;
using SiJwtAuth.Api.Dto.Response;
using SiJwtAuth.Application.Services;

namespace SiJwtAuth.Api.Controllers;

[Route("api/[controller]")]
[ApiController]
public class AuthController(
    IUserAuthService userAuthService,
    ITokenService tokenService
) : ControllerBase
{
    private const string BearerPrefix = "Bearer ";

    [AllowAnonymous]
    [HttpPut]
    public async Task<IActionResult> Register([FromBody] UserRegisterRequest userRegisterRequest)
    {
        var userId = await userAuthService.RegisterUserAsync(userRegisterRequest);
        if (userId == null)
        {
            return BadRequest();
        }

        return Ok();
    }

    [AllowAnonymous]
    [HttpPost]
    public async Task<IActionResult> Login([FromBody] UserLoginRequest userLoginRequest)
    {
        var oldAccessToken = AccessToken;
        if (oldAccessToken != null && tokenService.IsAccessTokenActual(oldAccessToken))
        {
            return Ok(new AccessTokenResponse(oldAccessToken));
        }

        var (user, ok) = await userAuthService.AuthenticateAsync(userLoginRequest.Username, userLoginRequest.Password);
        if (!ok)
        {
            return Ok("Bad credentials");
        }

        if (user == null)
        {
            return Ok("Wrong password");
        }

        var accessToken = await tokenService.GenerateAccessTokenAsync(user);
        return Ok(new AccessTokenResponse(accessToken));
    }

    private string? AccessToken => HttpContext.Request.Headers.Authorization
        .FirstOrDefault(it => it?.StartsWith(BearerPrefix) == true)?[BearerPrefix.Length..];
    

    [HttpGet]
    public async Task<IActionResult> Refresh()
    {
        var oldAccessToken = AccessToken;
        if (oldAccessToken == null)
        {
            return Unauthorized("No token");
        }

        var newAccessToken = await tokenService.TryRefreshAsync(oldAccessToken);
        if (newAccessToken == null)
        {
            return Unauthorized("Bad token");
        }

        return Ok(new AccessTokenResponse(newAccessToken));
    }
}