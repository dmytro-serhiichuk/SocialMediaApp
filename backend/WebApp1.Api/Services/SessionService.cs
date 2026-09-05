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
}