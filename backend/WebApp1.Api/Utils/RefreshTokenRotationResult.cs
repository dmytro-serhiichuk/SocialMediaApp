namespace WebApp1.Api.Utils;

public record RefreshTokenRotationResult(
    string Token,
    int UserId);