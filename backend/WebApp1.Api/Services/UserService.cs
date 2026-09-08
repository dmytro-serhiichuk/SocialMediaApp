using WebApp1.Api.Data;
using WebApp1.Api.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Npgsql;
using WebApp1.Api.DTOs;
using WebApp1.Api.Extensions;
using WebApp1.Api.Exceptions;
using WebApp1.Api.Utils;
using Microsoft.Extensions.Options;
using WebApp1.Api.Configuration;
using WebApp1.Api.Services.Email;
using WebApp1.Api.Data.Constants;

namespace WebApp1.Api.Services;

public class UserService(
    AppDbContext dbContext,
    IPasswordHasher<User> userPasswordHasher, 
    IPasswordHasher<PendingRegistration> registerPasswordHasher, 
    CurrentUserService currentUser, 
    IOptions<ConfiramtionTokenOptions> confiramtionTokenOptions,
    IEmailService emailService)
{
    private readonly ConfiramtionTokenOptions confiramtionTokenOptions = confiramtionTokenOptions.Value;

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
        var login = loginRequest.Login.Trim();

        User? user;

        // TODO: add actual email validation
        if (login.Contains('@'))
        {
            user = await dbContext.Users.FirstOrDefaultAsync(u => u.Email == login);
        }
        else
        {
            user = await dbContext.Users.FirstOrDefaultAsync(u => u.UserTag == login);
        }

        if (user is null) return null;

        if (user.Status != AccountStatus.Active)
        {
            throw new AccountRecoveryInProgressException();
        }

        var passwordVerificationResult = userPasswordHasher.VerifyHashedPassword(
            user, user.PasswordHash, loginRequest.Password
        );

        if (passwordVerificationResult == PasswordVerificationResult.Failed)
        {
            return null;            
        }

        return user;
    }

    public async Task HandleRegisterRequest(RegisterRequest request, CancellationToken cancellationToken = default)
    {
        var errors = await ValidatePendingRegister(request.Email, request.UserTag, cancellationToken);
        if (errors.Count > 0) throw new ConflictException(errors);

        var token = OpaqueTokenManager.Generate();
        var tokenHash = OpaqueTokenManager.HashToken(token);

        var pendingRegistration = new PendingRegistration
        {
            Id = Guid.NewGuid(),
            Email = request.Email,
            UserName = request.UserName,
            UserTag = request.UserTag,
            TokenHash = tokenHash,
            ExpiresAt = DateTime.UtcNow.AddHours(confiramtionTokenOptions.EmailConfirmationLifetimeHours)
        };

        pendingRegistration.PasswordHash = registerPasswordHasher.HashPassword(
            pendingRegistration, 
            request.Password
        );

        dbContext.PendingRegistrations.Add(pendingRegistration);
        await dbContext.SaveChangesAsync(cancellationToken);

        await emailService.SendRegistrationConfirmationMessageAsync(
            request.Email, 
            token,
            cancellationToken
        );
    }

    public async Task<User> ConfirmRegister(ConfirmationRequest confirmation, CancellationToken cancellationToken = default)
    {
        var tokenHash = OpaqueTokenManager.HashToken(confirmation.Token);

        var req = await dbContext.PendingRegistrations
            .SingleOrDefaultAsync(x => x.TokenHash == tokenHash, cancellationToken);

        if (req == null)
        {
            throw new InvalidConfirmationTokenException();
        }
        else if (req.ExpiresAt <= DateTime.UtcNow)
        {
            dbContext.PendingRegistrations.Remove(req);
            await dbContext.SaveChangesAsync(cancellationToken);
            throw new InvalidConfirmationTokenException();
        }

        var errors = await ValidatePendingRegister(req.Email, req.UserTag, cancellationToken);
        if (errors.ContainsKey("email"))
        {
            dbContext.PendingRegistrations.Remove(req);
            await dbContext.SaveChangesAsync(cancellationToken);
        }
        if (errors.Count > 0) throw new ConflictException(errors);

        return await CreateFromPendingRequest(req, req.UserTag, cancellationToken);
    }

    public async Task<User> CompleteRegister(RegisterCompletionRequest completionRequest, CancellationToken cancellationToken = default)
    {
        var tokenHash = OpaqueTokenManager.HashToken(completionRequest.Token);

        var req = await dbContext.PendingRegistrations
            .SingleOrDefaultAsync(x => x.TokenHash == tokenHash, cancellationToken);

        if (req == null)
        {
            throw new InvalidConfirmationTokenException();
        }
        else if (req.ExpiresAt <= DateTime.UtcNow)
        {
            dbContext.PendingRegistrations.Remove(req);
            await dbContext.SaveChangesAsync(cancellationToken);
            throw new InvalidConfirmationTokenException();
        }

        var errors = await ValidatePendingRegister(req.Email, completionRequest.UserTag, cancellationToken);

        if (errors.ContainsKey("email"))
        {
            dbContext.PendingRegistrations.Remove(req);
            await dbContext.SaveChangesAsync(cancellationToken);
        }
        if (errors.Count > 0) throw new ConflictException(errors);

        return await CreateFromPendingRequest(req, completionRequest.UserTag, cancellationToken);
    }

    private async Task<Dictionary<string, string[]>> ValidatePendingRegister(string email, string tag, CancellationToken cancellationToken = default)
    {
        var existingUsers = await dbContext.Users
            .Where(u => u.Email == email || u.UserTag == tag)
            .ToListAsync(cancellationToken);

        var emailExists = existingUsers.Any(u => u.Email == email);
        var tagExists = existingUsers.Any(u => u.UserTag == tag);

        var errors = new Dictionary<string, string[]>();

        if (emailExists) errors["email"] = ["This email is already in use."];
        if (tagExists) errors["tag"] = ["This tag is already taken."];

        return errors;
    }

    private async Task<User> CreateFromPendingRequest(PendingRegistration request, string tag, CancellationToken cancellationToken = default)
    {
        var user = new User
        {
            UserName = request.UserName,
            UserTag = tag,
            Email = request.Email,
            PasswordHash = request.PasswordHash
        };

        dbContext.Users.Add(user);
        dbContext.PendingRegistrations.Remove(request);

        try
        {
            await dbContext.SaveChangesAsync(cancellationToken);
            return user;
        }
        catch (DbUpdateException ex) when (ex.TryGetUniqueViolation(out var pex))
        {
            switch(pex.ConstraintName)
            {
                case UserIndexes.Email:
                    throw new ConflictException(new Dictionary<string, string[]>
                    {
                       ["email"] = ["This email is already in use."]
                    });
                case UserIndexes.UserTag:
                    throw new ConflictException(new Dictionary<string, string[]>
                    {
                       ["tag"] = ["This tag is already taken."]
                    });
                default:
                    throw;
            }
        }
    }

    public async Task RequestPasswordReset(RequestPasswordResetRequest request, CancellationToken cancellationToken = default)
    {
        var user = await dbContext.Users
            .SingleOrDefaultAsync(x => x.Email == request.Email, cancellationToken);

        if (user is null) return;

        var passwordReset = await dbContext.PasswordResets
            .SingleOrDefaultAsync(x => x.UserId == user.Id, cancellationToken);

        if (passwordReset is null)
        {
            passwordReset = new PasswordReset
            {
                UserId = user.Id
            };
            dbContext.PasswordResets.Add(passwordReset);
        }

        var token = OpaqueTokenManager.Generate();
        var tokenHash = OpaqueTokenManager.HashToken(token);

        passwordReset.TokenHash = tokenHash;
        passwordReset.ExpiresAt = DateTime.UtcNow.AddMinutes(confiramtionTokenOptions.PasswordResetLifetimeMinutes);

        await dbContext.SaveChangesAsync(cancellationToken);

        await emailService.SendPasswordResetMessageAsync(request.Email, token, cancellationToken);
    }

    public async Task<User> ConfirmPasswordReset(ResetPasswordRequest request, CancellationToken cancellationToken = default)
    {
        var tokenHash = OpaqueTokenManager.HashToken(request.Token);

        var resetRequest = await dbContext.PasswordResets
            .Include(x => x.User)
            .SingleOrDefaultAsync(x => x.TokenHash == tokenHash, cancellationToken);

        if (resetRequest == null || resetRequest.ExpiresAt <= DateTime.UtcNow)
        {
            throw new InvalidConfirmationTokenException();
        }

        var user = resetRequest.User;

        // TODO: add checking if user is not suspend (if the feature is implemented)
        user.Status = AccountStatus.Active;

        user.PasswordHash = userPasswordHasher.HashPassword(user, request.NewPassword);

        dbContext.PasswordResets.Remove(resetRequest);
        await dbContext.SaveChangesAsync(cancellationToken);

        return user;
    }

    public async Task<User> StartRecoveryProcess(ConfirmationRequest confirmation)
    {
        var tokenHash = OpaqueTokenManager.HashToken(confirmation.Token);

        var securityRequest = await dbContext.NewLoginSecurityRequests
            .Include(x => x.User)
            .SingleOrDefaultAsync(x => x.TokenHash == tokenHash);

        if (securityRequest is null)
        {
            throw new InvalidConfirmationTokenException();
        }
        else if (securityRequest.ExpiresAt <= DateTime.UtcNow)
        {
            throw new ConfirmationTokenExpiredException();
        }

        if (securityRequest.User.Status != AccountStatus.Active || 
            securityRequest.User.Status != AccountStatus.RecoveryRequired)
        {
            throw new AccountRecoveryNotAvailableException();
        }

        securityRequest.User.Status = AccountStatus.RecoveryRequired;

        return securityRequest.User;
    }

    public async Task CompleteAccountRecovery(ResetPasswordRequest request, CancellationToken cancellationToken = default)
    {
        var tokenHash = OpaqueTokenManager.HashToken(request.Token);

        var securityRequest = await dbContext.NewLoginSecurityRequests
            .Include(x => x.User)
            .SingleOrDefaultAsync(x => x.TokenHash == tokenHash, cancellationToken);

        if (securityRequest is null)
        {
            throw new InvalidConfirmationTokenException();
        }
        else if (securityRequest.ExpiresAt <= DateTime.UtcNow)
        {
            throw new ConfirmationTokenExpiredException();
        }

        var user = securityRequest.User;

        if (user.Status != AccountStatus.RecoveryRequired)
        {
            throw new AccountRecoveryNotAvailableException();
        }

        user.Status = AccountStatus.Active;

        user.PasswordHash = userPasswordHasher.HashPassword(user, request.NewPassword);

        dbContext.NewLoginSecurityRequests.Remove(securityRequest);
        await dbContext.SaveChangesAsync(cancellationToken);
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