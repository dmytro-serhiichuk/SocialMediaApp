namespace SocialMediaApp.Api.Models;

public class PostSave
{
    public int PostId { get; set; }
    public Post Post { get; set; } = null!;

    public int UserId { get; set; }
    public User User { get; set; } = null!;

    public DateTimeOffset CreatedAt { get; set; }
}