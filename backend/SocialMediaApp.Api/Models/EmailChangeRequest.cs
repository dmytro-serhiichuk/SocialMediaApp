namespace SocialMediaApp.Api.Models;

public class EmailChangeRequest
{
    public int UserId { get; set; }
    public User User { get; set; } = null!;
    public string NewEmail { get; set; } = null!;
    public string TokenHash { get; set; } = null!;
    public DateTime ExpiresAt { get; set; }
}