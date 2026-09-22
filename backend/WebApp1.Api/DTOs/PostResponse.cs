namespace WebApp1.Api.DTOs;

public class PostResponse
{
    public int Id { get; set; }
    public UserResponse User { get; set; } = null!;
    public string Content { get; set; } = string.Empty;
    public DateTimeOffset CreatedAt { get; set; }
    public DateTimeOffset? UpdatedAt { get; set; }
    public int LikesCount { get; set; }
    public bool IsLikedByMe { get; set; }
    public List<TagResponse> Tags { get; set; } = null!;
    public bool IsSavedByMe { get; set; }
    public List<ImageResponse> Images { get; set; } = null!;
    public int CommentsCount { get; set; }
}