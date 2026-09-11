using Microsoft.EntityFrameworkCore;
using WebApp1.Api.Data;
using WebApp1.Api.DTOs;
using WebApp1.Api.Models;

namespace WebApp1.Api.Services;

public class UserService(
    AppDbContext dbContext,
    CurrentUserService currentUser
)
{
    public async Task<UserProfileResponse?> GetUserProfile(string tag)
    {
        var currentUserId = currentUser.UserId;

        return await dbContext.Users
            .Where(u => u.UserTag == tag)
            .Select(user => new UserProfileResponse
            {
                Id = user.Id,
                UserName = user.UserName,
                UserTag = user.UserTag,
                FollowingCount = user.Following.Count,
                FollowersCount = user.Followers.Count,
                IsFollowedByMe = currentUserId != null && user.Followers.Any(u => u.FollowerId  == currentUserId),
                IsFollowingMe  = currentUserId != null && user.Following.Any(u => u.FollowingId == currentUserId),
            })
            .FirstOrDefaultAsync();
    }

    public async Task FollowUser(int userId, int currentUserId)
    {
        if (userId == currentUserId) return;

        var alreadyFollowed = await dbContext.UserFollows
            .AnyAsync(x => x.FollowerId == currentUserId && x.FollowingId == userId);

        if (!alreadyFollowed)
        {
            dbContext.UserFollows.Add(new UserFollow
            {
                FollowerId = currentUserId,
                FollowingId = userId
            });
        }

        await dbContext.SaveChangesAsync();
    }

    public async Task UnfollowUser(int userId, int currentUserId)
    {
        if (userId == currentUserId) return;
        
        var userFollow = await dbContext.UserFollows
            .FirstOrDefaultAsync(x => x.FollowerId == currentUserId && x.FollowingId == userId);

        if (userFollow is not null)
        {
            dbContext.UserFollows.Remove(userFollow);
            await dbContext.SaveChangesAsync();
        }
    }

    public async Task<List<UserResponse>> FindUsersByTag(UserSearchRequest searchRequest, int limit = 5)
    {
        return await dbContext.Users
            .Where(u => EF.Functions.Like(u.UserTag, $"%{searchRequest.Query}%"))
            .OrderByDescending(u => u.Followers.Count)
            .Take(limit)
            .Select(u => UserResponse.CreateFromUser(u))
            .ToListAsync();
    }
}