namespace SiJwtAuth.Api.Dto.Command;

public record UserRegisterRequest(
    string Username,
    string Email,
    string Password
);