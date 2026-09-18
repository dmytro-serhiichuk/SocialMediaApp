namespace WebApp1.Api.DTOs;

public class CommentResponse
{
    public int Id { get; set; }
    public UserResponse User { get; set; } = null!;
    public string Content { get; set; } = string.Empty;
    public DateTimeOffset CreatedAt { get; set; }
    public DateTimeOffset? UpdatedAt { get; set; }
}