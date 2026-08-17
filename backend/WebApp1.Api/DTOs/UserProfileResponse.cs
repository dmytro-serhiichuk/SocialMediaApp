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

    public static UserProfileResponse CreateFromUser(User user, int? currentUserId)
    {
        return new UserProfileResponse
        {
            Id = user.Id,
            UserName = user.UserName,
            UserTag = user.UserTag,
            FollowingCount = user.Following.Count,
            FollowersCount = user.Followers.Count,
            IsFollowedByMe = currentUserId != null && user.Followers.Any(u => u.FollowerId  == currentUserId),
            IsFollowingMe  = currentUserId != null && user.Following.Any(u => u.FollowingId == currentUserId),
        };
    }
}