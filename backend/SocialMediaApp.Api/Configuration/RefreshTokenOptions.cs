namespace SocialMediaApp.Api.Configuration;

public class RefreshTokenOptions
{
    public const string SectionName = "RefreshToken";

    public int TokenLifetimeDays { get; set; }
    public int GraceTimeSeconds { get; set; }
    public string TokenCookieName { get; set; } = "refresh_token";
}