namespace SocialMediaApp.Api.Models;

public class Comment
{
    public int Id { get; set; }

    public int UserId { get; set; }
    public User User { get; set; } = null!;

    public int PostId { get; set; }
    public Post Post { get; set; } = null!;

    public string Content { get; set; } = null!;

    public int? ParentCommentId { get; set; } = null;
    public Comment? ParentComment { get; set; } = null;

    public int? ReplyToUserId { get; set; }
    public User? ReplyToUser { get; set; }

    public ICollection<Comment> Replies { get; set; } = [];
    public ICollection<CommentLike> Likes { get; set; } = [];

    public DateTimeOffset CreatedAt { get; set; }
    public DateTimeOffset? UpdatedAt { get; set; } = null;
}