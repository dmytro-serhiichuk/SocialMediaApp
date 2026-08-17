namespace WebApp1.Api.Models;

public class User
{
    public int Id { get; set; }
    public string UserName { get; set; } = string.Empty;
    public string UserTag { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string PasswordHash { get; set; } = string.Empty;
    public ICollection<PostLike> LikedPosts { get; set; } = [];
    public ICollection<UserFollow> Following { get; set; } = [];
    public ICollection<UserFollow> Followers { get; set; } = [];
}