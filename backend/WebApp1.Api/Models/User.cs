namespace WebApp1.Api.Models;

public class User
{
    public int Id { get; set; }
    public string UserName { get; set; } = string.Empty;
    public string UserTag { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string? PasswordHash { get; set; } = null;
    public AccountStatus Status { get; set; }
    public ICollection<ExternalLogin> ExternalLogins { get; set; } = [];
    public ICollection<Session> Sessions { get; set; } = [];
    public ICollection<Post> Posts { get; set; } = [];
    public ICollection<PostLike> LikedPosts { get; set; } = [];
    public ICollection<CommentLike> LikedComments { get; set; } = [];
    public ICollection<PostSave> SavedPosts { get; set; } = [];
    public ICollection<UserFollow> Following { get; set; } = [];
    public ICollection<UserFollow> Followers { get; set; } = [];
    public ICollection<Comment> Comments { get; set; } = [];
}

public enum AccountStatus
{
    Active,
    RecoveryRequired
}