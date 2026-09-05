using WebApp1.Api.Data;
using WebApp1.Api.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Npgsql;
using WebApp1.Api.DTOs;
using WebApp1.Api.Extensions;

namespace WebApp1.Api.Services;

public class UserService(AppDbContext dbContext, IPasswordHasher<User> passwordHasher, CurrentUserService currentUser)
{
    public async Task<User?> GetById(int id)
    {
        return await dbContext.Users.FirstOrDefaultAsync(u => u.Id == id);
    }

    public async Task<User?> GetByEmail(string email)
    {
        return await dbContext.Users.FirstOrDefaultAsync(u => u.Email == email);
    }
    public async Task<User?> GetByUserTag(string tag)
    {
        return await dbContext.Users.FirstOrDefaultAsync(u => u.UserTag == tag);
    }

    public async Task<User?> Login(LoginRequest loginRequest)
    {
        // TODO: send email with a message about new login

        var login = loginRequest.Login.Trim();

        User? user;

        // TODO: add actual email validation
        if (login.Contains('@'))
            user = await dbContext.Users.FirstOrDefaultAsync(u => u.Email == login);
        else
            user = await dbContext.Users.FirstOrDefaultAsync(u => u.UserTag == login);

        if (user is null)
            return null;

        var passwordVerificationResult = passwordHasher.VerifyHashedPassword(
            user, user.PasswordHash, loginRequest.Password
        );

        if (passwordVerificationResult == PasswordVerificationResult.Failed)
            return null;

        return user;
    }

    public async Task<User?> Create(RegisterRequest registerRequest)
    {
        // TODO: send confirmation letter to user's email
        try
        {
            var exists = await dbContext.Users
                        .AnyAsync(u => u.Email == registerRequest.Email);

            if (exists)
                return null;

            var user = new User
            {
                UserName = registerRequest.UserName,
                UserTag = registerRequest.UserTag,
                Email = registerRequest.Email
            };
            user.PasswordHash = passwordHasher.HashPassword(user, registerRequest.Password);

            dbContext.Users.Add(user);
            await dbContext.SaveChangesAsync();
            return user;
        }
        catch (DbUpdateException ex) when (ex.IsUniqueViolation())
        {
            return null;
        }
    }

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