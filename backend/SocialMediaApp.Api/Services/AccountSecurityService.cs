using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using SocialMediaApp.Api.Configuration;
using SocialMediaApp.Api.Data;
using SocialMediaApp.Api.DTOs;
using SocialMediaApp.Api.Exceptions;
using SocialMediaApp.Api.Models;
using SocialMediaApp.Api.Services.Email;
using SocialMediaApp.Api.Utils;

namespace SocialMediaApp.Api.Services;

public class AccountSecurityService(
    AppDbContext dbContext,
    IOptions<ConfirmationTokenOptions> ConfirmationTokenOptions,
    IEmailService emailService,
    IPasswordHasher<User> userPasswordHasher)
{
    private readonly ConfirmationTokenOptions ConfirmationTokenOptions = ConfirmationTokenOptions.Value;

    public async Task RequestPasswordReset(RequestPasswordResetRequest request, CancellationToken cancellationToken = default)
    {
        var user = await dbContext.Users
            .SingleOrDefaultAsync(x => x.Email == request.Email, cancellationToken);

        if (user is null) return;

        if (request.RequestType == RequestPasswordResetType.Create && user.PasswordHash is not null) return;

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
        passwordReset.ExpiresAt = DateTime.UtcNow.AddMinutes(ConfirmationTokenOptions.PasswordResetLifetimeMinutes);

        await dbContext.SaveChangesAsync(cancellationToken);

        if (request.RequestType == RequestPasswordResetType.Reset)
        {
            await emailService.SendPasswordResetMessageAsync(request.Email, token, cancellationToken);
        }
        else
        {
            await emailService.SendPasswordCreateMessageAsync(request.Email, token, cancellationToken);
        }
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

        if (securityRequest is null || securityRequest.ExpiresAt <= DateTime.UtcNow)
        {
            throw new InvalidConfirmationTokenException();
        }

        if (securityRequest.User.Status != AccountStatus.Active && 
            securityRequest.User.Status != AccountStatus.RecoveryRequired)
        {
            throw new AccountRecoveryNotAvailableException();
        }

        securityRequest.User.Status = AccountStatus.RecoveryRequired;

        await dbContext.SaveChangesAsync();

        return securityRequest.User;
    }

    public async Task CompleteAccountRecovery(ResetPasswordRequest request, CancellationToken cancellationToken = default)
    {
        var tokenHash = OpaqueTokenManager.HashToken(request.Token);

        var securityRequest = await dbContext.NewLoginSecurityRequests
            .Include(x => x.User)
            .SingleOrDefaultAsync(x => x.TokenHash == tokenHash, cancellationToken);

        if (securityRequest is null || securityRequest.ExpiresAt <= DateTime.UtcNow)
        {
            throw new InvalidConfirmationTokenException();
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
}