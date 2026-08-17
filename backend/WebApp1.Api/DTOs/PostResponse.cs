using WebApp1.Api.Models;

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

    public static PostResponse CreateFromPost(Post post, int? currentUserId)
    {
        return new PostResponse
            {
                Id = post.Id,
                Content = post.Content,
                CreatedAt = post.CreatedAt,
                UpdatedAt = post.UpdatedAt,
                User = UserResponse.CreateFromUser(post.User),
                LikesCount = post.Likes.Count,
                IsLikedByMe = currentUserId != null && post.Likes.Any(l => l.UserId == currentUserId)
            };
    }
}