using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using WebApp1.Api.Configuration;
using WebApp1.Api.Data;
using WebApp1.Api.DTOs;
using WebApp1.Api.Exceptions;
using WebApp1.Api.Extensions;
using WebApp1.Api.Models;
using WebApp1.Api.Services.Email;
using WebApp1.Api.Utils;

namespace WebApp1.Api.Services;

public class SettingsService(AppDbContext dbContext, IOptions<EmailChangeOptions> options, IEmailService emailService, IPasswordHasher<User> passwordHasher)
{
    private readonly EmailChangeOptions emailChangeOptions = options.Value;

    public async Task<SettingsDataResponse?> GetData(int userId, int sessionId)
    {
        return await dbContext.Users
            .Where(x => x.Id == userId)
            .Select(x => new SettingsDataResponse
            {
                Id = x.Id,
                UserName = x.UserName,
                UserTag = x.UserTag,
                Email = x.Email,
                Sessions = x.Sessions
                    .OrderByDescending(s => s.Id == sessionId)
                    .ThenByDescending(s => s.LastUsedAt)
                    .Select(s => new SessionResponse
                    {
                        Id = s.Id,
                        FirstLogin = s.CreatedAt,
                        LastLogin = s.LastUsedAt,
                        DeviceInfo = s.LastDeviceInfo,
                        Ip = s.LastIpAddress,
                        IsCurrentSession = s.Id == sessionId
                    }).ToList()
            })
            .FirstOrDefaultAsync();
    }

    public async Task UpdateProfile(int userId, SettingsProfileRequest profileRequest)
    {
        var user = await dbContext.Users.SingleAsync(x => x.Id == userId);

        user.UserName = profileRequest.UserName;
        user.UserTag = profileRequest.UserTag;

        try
        {
            await dbContext.SaveChangesAsync();
        }
        catch (DbUpdateException ex) when (ex.IsUniqueViolation())
        {
            throw new UserTagAlreadyTakenException();
        }
    }

    public async Task RequestEmailChange(int userId, DTOs.EmailChangeRequest emailChangeRequest)
    {
        var isEmailAlreadyUsed = await dbContext.Users.AnyAsync(x => x.Email == emailChangeRequest.NewEmail);
        if (isEmailAlreadyUsed)
        {
            throw new EmailAlreadyExistsException();
        }

        var req = await dbContext.EmailChangeRequests.SingleOrDefaultAsync(x => x.UserId == userId);

        var token = OpaqueTokenManager.Generate();
        var tokenHash = OpaqueTokenManager.HashToken(token);

        if (req is null)
        {
            req = new Models.EmailChangeRequest
            {
                UserId = userId
            };

            dbContext.EmailChangeRequests.Add(req);
        }

        req.NewEmail = emailChangeRequest.NewEmail;
        req.ExpiresAt = DateTime.UtcNow.AddMinutes(emailChangeOptions.TokenLifetimeMinutes);
        req.TokenHash = tokenHash;
        
        await dbContext.SaveChangesAsync();

        // TODO: remove
        System.Console.WriteLine(token);

        await emailService.SendEmailConfirmationMessageAsync(emailChangeRequest.NewEmail, token);
    }

    public async Task ConfirmEmailChange(EmailChangeConfirmation confirmation)
    {
        var tokenHash = OpaqueTokenManager.HashToken(confirmation.Token);

        var req = await dbContext.EmailChangeRequests
            .Include(x => x.User)
            .SingleOrDefaultAsync(x => x.TokenHash == tokenHash);

        if (req == null || req.ExpiresAt <= DateTime.UtcNow)
        {
            throw new InvalidConfirmationTokenException();
        }

        var isEmailAlreadyUsed = await dbContext.Users.AnyAsync(x => x.Email == req.NewEmail);
        if (isEmailAlreadyUsed)
        {
            throw new EmailAlreadyExistsException();
        }

        req.User.Email = req.NewEmail;

        dbContext.EmailChangeRequests.Remove(req);

        try
        {
            await dbContext.SaveChangesAsync();
        }
        catch (DbUpdateException ex) when (ex.IsUniqueViolation())
        {
            throw new EmailAlreadyExistsException();
        }
    }

    public async Task ChangePassword(int userId, PasswordChangeRequest passwordChangeRequest)
    {
        var user = await dbContext.Users.SingleAsync(x => x.Id == userId);

        var passwordVerificationResult = passwordHasher.VerifyHashedPassword(
            user, user.PasswordHash, passwordChangeRequest.OldPassword
        );

        if (passwordVerificationResult == PasswordVerificationResult.Failed)
        {
            throw new IncorrectPasswordException();
        }

        user.PasswordHash = passwordHasher.HashPassword(user, passwordChangeRequest.NewPassword);
        
        await dbContext.SaveChangesAsync();

        // TODO: send an email with a recovery link
    }
}