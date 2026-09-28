namespace WebApp1.Api.DTOs;

public class CommentResponse
{
    public int Id { get; set; }
    public UserResponse User { get; set; } = null!;
    public string Content { get; set; } = string.Empty;
    public DateTimeOffset CreatedAt { get; set; }
    public DateTimeOffset? UpdatedAt { get; set; }
    public string? ReplyToUserTag { get; set; }
    public int RepliesCount { get; set; } = 0;
    public int LikesCount { get; set; } = 0;
    public bool IsLikedByMe { get; set; } = false;
}