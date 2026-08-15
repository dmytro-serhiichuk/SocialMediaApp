using WebApp1.Api.Models;

namespace WebApp1.Api.DTOs;

public class PostResponse
{
    public int Id { get; set; }
    public UserResponse User { get; set; } = null!;
    public string Content { get; set; } = string.Empty;
    public DateTimeOffset CreatedAt { get; set; }
    public DateTimeOffset? UpdatedAt { get; set; }

    public static PostResponse CreateFromPost(Post post)
    {
        return new PostResponse
        {
            Id = post.Id,
            Content = post.Content,
            CreatedAt = post.CreatedAt,
            UpdatedAt = post.UpdatedAt,
            User = UserResponse.CreateFromUser(post.User)
        };
    }
}