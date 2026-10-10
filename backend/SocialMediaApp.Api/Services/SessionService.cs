using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using UAParser;
using SocialMediaApp.Api.Configuration;
using SocialMediaApp.Api.Data;
using SocialMediaApp.Api.Exceptions;
using SocialMediaApp.Api.Models;
using SocialMediaApp.Api.Services.Email;
using SocialMediaApp.Api.Utils;

namespace SocialMediaApp.Api.Services;

public class SessionService(
    AppDbContext dbContext, 
    IOptions<RefreshTokenOptions> tokenOptions,
    IOptions<ConfiramtionTokenOptions> confiramtionTokenOptions,
    IEmailService emailService)
{
    private readonly RefreshTokenOptions tokenOptions = tokenOptions.Value;
    private readonly ConfiramtionTokenOptions confiramtionTokenOptions = confiramtionTokenOptions.Value;

    public async Task<SessionCreationResult> CreateSession(User user, string? userAgent, string? ip)
    {
        var refreshToken = OpaqueTokenManager.Generate();
        var refreshTokenHash = OpaqueTokenManager.HashToken(refreshToken);

        var now = DateTimeOffset.UtcNow;

        var session = new Session
        {
            UserId = user.Id,

            RefreshTokenHash = refreshTokenHash,
            RefreshTokenExpiresAt = now.AddDays(tokenOptions.TokenLifetimeDays),

            PreviousTokenGraceUntil = now.AddSeconds(tokenOptions.GraceTimeSeconds),

            CreatedAt = now,
            LastUsedAt = now,

            LastDeviceInfo = userAgent != null ? ParseUserAgent(userAgent) : null,
            LastIpAddress = ip,

            State = SessionState.Active
        };

        dbContext.Sessions.Add(session);

        var token = OpaqueTokenManager.Generate();
        var tokenHash = OpaqueTokenManager.HashToken(token);

        dbContext.NewLoginSecurityRequests.Add(new NewLoginSecurityRequest
        {
            UserId = user.Id,
            Session = session,
            TokenHash = tokenHash,
            ExpiresAt = now.AddMinutes(confiramtionTokenOptions.NewLoginTokenLifetimeMinutes)
        });

        await dbContext.SaveChangesAsync();

        await emailService.SendNewLoginNotificationMessageAsync(
            user.Email,
            session,
            token
        );

        return new SessionCreationResult(refreshToken, session);
    }

    private string ParseUserAgent(string userAgent)
    {
        var parser = Parser.GetDefault();

        var clientInfo = parser.Parse(userAgent);

        var browser = clientInfo.UA.Family;
        var browserVersion = clientInfo.UA.Major;

        var os = clientInfo.OS.Family;
        var osVersion = clientInfo.OS.Major;

        return $"{browser} {browserVersion}, {os} {osVersion}";
    }

    public async Task LogoutSession(string refreshToken)
    {
        var tokenHash = OpaqueTokenManager.HashToken(refreshToken);

        var session = await dbContext.Sessions
            .SingleOrDefaultAsync(s => s.RefreshTokenHash == tokenHash);

        if (session != null && session.State == SessionState.Active)
        {
            session.State = SessionState.LoggedOut;
            await dbContext.SaveChangesAsync();
        }
    }

    public async Task<SessionCreationResult?> RotateRefreshToken(string refreshToken, string? userAgent, string? ip)
    {
        var tokenHash = OpaqueTokenManager.HashToken(refreshToken);

        var session = await dbContext.Sessions
            .SingleOrDefaultAsync(s => s.RefreshTokenHash == tokenHash);

        if (session is not null)
        {
            if (session.State == SessionState.Active)
            {
                return new SessionCreationResult(await RotateAsync(session, userAgent, ip), session);
            }
            else
            {
                return null;
            }
        }

        var reused = await dbContext.Sessions
            .SingleOrDefaultAsync(s => s.PreviousRefreshTokenHash == tokenHash);

        if (reused is null || reused.State != SessionState.Active)
        {
            return null;
        }
        else if (reused.PreviousTokenGraceUntil < DateTime.UtcNow)
        {
            reused.State = SessionState.ReuseDetected;
            await dbContext.SaveChangesAsync();
            return null;
        }
        
        return new SessionCreationResult(await RotateAsync(reused, userAgent, ip), reused);
    }

    private async Task<string> RotateAsync(Session session, string? userAgent, string? ip)
    {
        var refreshToken = OpaqueTokenManager.Generate();
        var refreshTokenHash = OpaqueTokenManager.HashToken(refreshToken);

        var now = DateTimeOffset.UtcNow;

        session.PreviousRefreshTokenHash = session.RefreshTokenHash;
        session.RefreshTokenHash = refreshTokenHash;
        session.RefreshTokenExpiresAt = now.AddDays(tokenOptions.TokenLifetimeDays);
        session.PreviousTokenGraceUntil = now.AddSeconds(tokenOptions.GraceTimeSeconds);
        session.LastUsedAt = now;
        session.LastDeviceInfo = userAgent != null ? ParseUserAgent(userAgent) : null;
        session.LastIpAddress = ip;

        await dbContext.SaveChangesAsync();

        return refreshToken;
    }

    public async Task<bool> TerminateSessionById(int userId, int sessionId)
    {
        var session = await dbContext.Sessions
            .SingleOrDefaultAsync(s => s.Id == sessionId && s.UserId == userId);

        if (session is null)
        {
            return false;
        }

        if (session.State != SessionState.Active)
        {
            throw new InvalidSessionStateException();
        }

        session.State = SessionState.Terminated;
        await dbContext.SaveChangesAsync();
        return true;
    }

    public async Task TerminateAllSessionsButCurrent(int userId, int sessionId)
    {
        var sessions = await dbContext.Sessions
            .Where(s => s.Id != sessionId && s.UserId == userId && s.State == SessionState.Active)
            .ToListAsync();

        sessions.ForEach(s => s.State = SessionState.Terminated);
        await dbContext.SaveChangesAsync();
    }

    public async Task TerminateAllSessions(int userId)
    {
        var sessions = await dbContext.Sessions
            .Where(s => s.UserId == userId && s.State == SessionState.Active)
            .ToListAsync();

        sessions.ForEach(s => s.State = SessionState.Terminated);
        await dbContext.SaveChangesAsync();
    }
}