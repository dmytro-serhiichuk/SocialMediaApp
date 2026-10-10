using SocialMediaApp.Api.Data;
using SocialMediaApp.Api.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using SocialMediaApp.Api.DTOs;
using SocialMediaApp.Api.Extensions;
using SocialMediaApp.Api.Exceptions;
using SocialMediaApp.Api.Utils;
using Microsoft.Extensions.Options;
using SocialMediaApp.Api.Configuration;
using SocialMediaApp.Api.Services.Email;
using SocialMediaApp.Api.Data.Constants;

namespace SocialMediaApp.Api.Services;

public class AuthService(
    AppDbContext dbContext,
    IPasswordHasher<User> userPasswordHasher, 
    IPasswordHasher<PendingRegistration> registerPasswordHasher, 
    IOptions<ConfirmationTokenOptions> ConfirmationTokenOptions,
    IEmailService emailService)
{
    private readonly ConfirmationTokenOptions ConfirmationTokenOptions = ConfirmationTokenOptions.Value;

    public async Task<User?> GetById(int id)
    {
        return await dbContext.Users.FirstOrDefaultAsync(u => u.Id == id);
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

        if (user is null || user.PasswordHash is null) return null;

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
            ExpiresAt = DateTime.UtcNow.AddHours(ConfirmationTokenOptions.EmailConfirmationLifetimeHours)
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
            PasswordHash = request.PasswordHash,
            Status = AccountStatus.Active
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

    public async Task<ExternalLoginResult> HandleExternalLogin(Utils.ExternalLoginInfo externalLoginInfo)
    {
        var existingLogin = await dbContext.ExternalLogins
            .Include(x => x.User)
            .SingleOrDefaultAsync(x => 
                x.Provider == externalLoginInfo.Provider && 
                x.ProviderUserId == externalLoginInfo.ProviderUserId);

        if (existingLogin is not null)
        {
            return new ExternalLoginResult.Success(existingLogin.User);
        }

        var existingUser = await dbContext.Users
            .SingleOrDefaultAsync(x => x.Email == externalLoginInfo.Email);

        if (existingUser is not null)
        {
            return new ExternalLoginResult.EmailAlreadyTaken(externalLoginInfo.Email);
        }

        var user = new User
        {
            UserName = externalLoginInfo.Name,
            UserTag = UserTagGenerator.GenerateTag(externalLoginInfo.Name),
            Email = externalLoginInfo.Email,
            PasswordHash = null,
            Status = AccountStatus.Active
        };

        var login = new ExternalLogin
        {
            User = user,
            Provider = externalLoginInfo.Provider,
            ProviderUserId = externalLoginInfo.ProviderUserId
        };

        dbContext.Users.Add(user);
        dbContext.ExternalLogins.Add(login);

        for (var attempt = 0; attempt < 5; attempt++)
        {
            try
            {
                await dbContext.SaveChangesAsync();
                return new ExternalLoginResult.Success(user);
            }
            catch (DbUpdateException ex) when (ex.TryGetUniqueViolation(out var pex))
            {
                switch(pex.ConstraintName)
                {
                    case UserIndexes.Email:
                        return new ExternalLoginResult.EmailAlreadyTaken(externalLoginInfo.Email);
                    case UserIndexes.UserTag:
                        user.UserTag = UserTagGenerator.GenerateTag(externalLoginInfo.Name);
                        break;
                    default:
                        throw;
                }
            }
        }

        throw new InvalidOperationException("Failed to generate a unique user tag.");
    }

    public async Task<ConnectExternalLoginResult> ConnectExternalLogin(int userId, Utils.ExternalLoginInfo info)
    {
        var existingLogin = await dbContext.ExternalLogins
            .SingleOrDefaultAsync(x => x.Provider == info.Provider && x.ProviderUserId == info.ProviderUserId);

        if (existingLogin is not null)
        {
            return existingLogin.UserId == userId
                ? new ConnectExternalLoginResult.AlreadyLinkedToThisAccount()
                : new ConnectExternalLoginResult.AlreadyLinkedToAnotherAccount();
        }

        dbContext.ExternalLogins.Add(new ExternalLogin
        {
            UserId = userId,
            Provider = info.Provider,
            ProviderUserId = info.ProviderUserId
        });

        await dbContext.SaveChangesAsync();
        return new ConnectExternalLoginResult.Success();
    }
}