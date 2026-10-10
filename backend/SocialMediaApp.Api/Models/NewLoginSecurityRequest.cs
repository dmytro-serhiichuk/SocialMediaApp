namespace SocialMediaApp.Api.Models;

public class NewLoginSecurityRequest
{
    public int Id { get; set; }
    public int UserId { get; set; }
    public User User { get; set; } = null!;
    public int? SessionId { get; set; }
    public Session? Session { get; set; } = null!;
    public string TokenHash { get; set; } = null!;
    public DateTimeOffset ExpiresAt { get; set; }
}