namespace WebApp1.Api.Models;

public class NewLoginSecurityRequest
{
    public int UserId { get; set; }
    public User User { get; set; } = null!;
    public string TokenHash { get; set; } = null!;
    public DateTimeOffset ExpiresAt { get; set; }
}