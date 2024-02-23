namespace SiJwtAuth.Dto.Response;

public record UserLoginResponse(string AccessToken, string RefreshToken);