using Microsoft.EntityFrameworkCore;
using WebApp1.Api.Data;
using WebApp1.Api.DTOs;
using WebApp1.Api.Models;
using WebApp1.Api.Utils;

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

    public async Task<List<UserSearchResponse>> FindUsersByTag(UserSearchRequest searchRequest, int limit = 5)
    {
        return await dbContext.Users
            .Where(u => EF.Functions.Like(u.UserTag, $"%{searchRequest.Query}%"))
            .OrderByDescending(u => u.Followers.Count)
            .Take(limit)
            .Select(u => UserSearchResponse.CreateFromUser(u))
            .ToListAsync();
    }

    public async Task<PaginationResponse<UserResponse>> GetUserFollowers(PaginationRequest paginationRequest, string tag)
    {
        IQueryable<UserFollow> query = dbContext.Users
            .Where(x => x.UserTag == tag)
            .SelectMany(x => x.Followers)
            .OrderBy(x => x.FollowingId);


        return await GetUsersByQuery(query, paginationRequest);
    }

    public async Task<PaginationResponse<UserResponse>> GetUserFollowing(PaginationRequest paginationRequest, string tag)
    {
        IQueryable<UserFollow> query = dbContext.Users
            .Where(x => x.UserTag == tag)
            .SelectMany(x => x.Following)
            .OrderBy(x => x.FollowerId);

        return await GetUsersByQuery(query, paginationRequest);
    }

    public async Task<PaginationResponse<UserResponse>> GetUsersByQuery(IQueryable<UserFollow> query, PaginationRequest paginationRequest)
    {
        var cursor = CursorEncoder.Decode<UserPaginationCursor>(paginationRequest.Cursor);

        if (cursor is not null)
        {
            query = query.Where(x => x.FollowingId < cursor.Id);
        }

        int? currentUserId = currentUser.UserId;

        var users = await query
            .Take(paginationRequest.Limit + 1)
            .Select(x => new UserResponse
            {
                Id = x.Following.Id,
                UserName = x.Following.UserName,
                UserTag = x.Following.UserTag,
                IsFollowedByMe = currentUserId != null 
                    && x.Following.Followers.Any(y => y.FollowerId == currentUserId),
                IsFollowingMe  = currentUserId != null 
                    && x.Following.Following.Any(y => y.FollowingId == currentUserId),
            }).ToListAsync();

        var hasMore = users.Count > paginationRequest.Limit;
        if (hasMore) users.RemoveAt(users.Count - 1);

        string? nextCursor = null;
        if (hasMore && users.Count > 0)
        {
            var last = users[^1];
            nextCursor = CursorEncoder.Encode(new UserPaginationCursor(last.Id));
        }

        return new PaginationResponse<UserResponse>
        {
            Items = users,
            HasMore = hasMore,
            NextCursor = nextCursor
        };
    }
}