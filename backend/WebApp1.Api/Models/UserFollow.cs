namespace WebApp1.Api.Models;

public class UserFollow
{
    public int FollowerId { get; set; }
    public User Follower { get; set; } = null!;

    public int FollowingId { get; set; }
    public User Following { get; set; } = null!;
}