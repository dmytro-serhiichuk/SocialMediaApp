using WebApp1.Api.Models;

namespace WebApp1.Api.DTOs;

public class UserProfileResponse
{
    public int Id { get; set; }
    public string UserName { get; set; } = string.Empty;
    public string UserTag { get; set; } = string.Empty;
    public int FollowingCount { get; set; }
    public int FollowersCount { get; set; }
    public bool IsFollowedByMe { get; set; }
    public bool IsFollowingMe { get; set; }
}