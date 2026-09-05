using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using WebApp1.Api.Configuration;
using WebApp1.Api.Data;
using WebApp1.Api.Models;
using WebApp1.Api.Utils;

namespace WebApp1.Api.Services;

public class SessionService(AppDbContext dbContext, IOptions<RefreshTokenOptions> tokenOptions)
{
    private readonly RefreshTokenOptions tokenOptions = tokenOptions.Value;

    public async Task<string> CreateSession(int userId, string? userAgent, string? ip)
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

            LastUserAgent = userAgent,
            LastIpAddress = ip
        };

        dbContext.Sessions.Add(session);
        await dbContext.SaveChangesAsync();

        return refreshToken;
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

    public async Task<RefreshTokenRotationResult?> RotateRefreshToken(string refreshToken, string? userAgent, string? ip)
    {
        var tokenHash = OpaqueTokenManager.HashToken(refreshToken);

        var session = await dbContext.Sessions
            .SingleOrDefaultAsync(s => s.RefreshTokenHash == tokenHash);

        if (session is not null)
        {
            return new RefreshTokenRotationResult(await RotateAsync(session, userAgent, ip), session.UserId);
        }

        var reused = await dbContext.Sessions
            .SingleOrDefaultAsync(s => s.PreviousRefreshTokenHash == tokenHash);

        if (reused is null || reused.PreviousTokenGraceUntil < DateTime.UtcNow)
        {
            return null;
        }
        
        return new RefreshTokenRotationResult(await RotateAsync(reused, userAgent, ip), reused.UserId);
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
        session.LastUserAgent = userAgent;
        session.LastIpAddress = ip;

        await dbContext.SaveChangesAsync();

        return refreshToken;
    }
}