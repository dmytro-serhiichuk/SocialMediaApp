public class JwtOptions
{
    public const string SectionName = "Jwt";

    public string SecretKey { get; set; } = null!;
    public string Issuer { get; set; } = null!;
    public string Audience { get; set; } = null!;
    public int AccessTokenLifetimeMinutes { get; set; }
    public string AccessTokenCookieName { get; set; } = "access_token";
}