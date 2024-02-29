namespace SiJwtAuth.Api.Dto.Command;

public record UserLoginRequest(
    string Username,
    string Password
);