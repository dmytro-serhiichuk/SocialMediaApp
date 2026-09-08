using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using UAParser;
using WebApp1.Api.Configuration;
using WebApp1.Api.Data;
using WebApp1.Api.Models;
using WebApp1.Api.Utils;

namespace WebApp1.Api.Services;

public class SessionService(AppDbContext dbContext, IOptions<RefreshTokenOptions> tokenOptions)
{
    private readonly RefreshTokenOptions tokenOptions = tokenOptions.Value;

    public async Task<SessionCreationResult> CreateSession(int userId, string? userAgent, string? ip)
    {
        var refreshToken = OpaqueTokenManager.Generate();
        var refreshTokenHash = OpaqueTokenManager.HashToken(refreshToken);

        var now = DateTimeOffset.UtcNow;

        var session = new Session
        {
            UserId = userId,

            RefreshTokenHash = refreshTokenHash,
            RefreshTokenExpiresAt = now.AddDays(tokenOptions.TokenLifetimeDays),

            PreviousTokenGraceUntil = now.AddSeconds(tokenOptions.GraceTimeSeconds),

            CreatedAt = now,
            LastUsedAt = now,

            LastDeviceInfo = userAgent != null ? ParseUserAgent(userAgent) : null,
            LastIpAddress = ip
        };

        dbContext.Sessions.Add(session);
        await dbContext.SaveChangesAsync();

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

    public async Task DeleteByToken(string refreshToken)
    {
        var tokenHash = OpaqueTokenManager.HashToken(refreshToken);

        var session = await dbContext.Sessions
            .SingleOrDefaultAsync(s => s.RefreshTokenHash == tokenHash);

        if (session != null)
        {
            dbContext.Sessions.Remove(session);
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
            return new SessionCreationResult(await RotateAsync(session, userAgent, ip), session);
        }

        var reused = await dbContext.Sessions
            .SingleOrDefaultAsync(s => s.PreviousRefreshTokenHash == tokenHash);

        if (reused is null || reused.PreviousTokenGraceUntil < DateTime.UtcNow)
        {
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

        dbContext.Sessions.Remove(session);
        await dbContext.SaveChangesAsync();
        return true;
    }

    public async Task TerminateAllSessionsButCurrent(int userId, int sessionId)
    {
        var sessions = await dbContext.Sessions
            .Where(s => s.Id != sessionId && s.UserId == userId)
            .ToListAsync();

        dbContext.Sessions.RemoveRange(sessions);
        await dbContext.SaveChangesAsync();
    }

    public async Task TerminateAllSessions(int userId)
    {
        var sessions = await dbContext.Sessions
            .Where(s => s.UserId == userId)
            .ToListAsync();

        dbContext.Sessions.RemoveRange(sessions);
        await dbContext.SaveChangesAsync();
    }
}