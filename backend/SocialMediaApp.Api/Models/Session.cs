using System.Text.Json.Serialization;

namespace SocialMediaApp.Api.Models;

public class Session
{
    public int Id { get; set; }
    public int UserId { get; set; }
    public User User { get; set; } = null!;

    public string RefreshTokenHash { get; set; } = string.Empty;
    public DateTimeOffset RefreshTokenExpiresAt { get; set; }

    public string? PreviousRefreshTokenHash { get; set; }
    public DateTimeOffset? PreviousTokenGraceUntil { get; set; }

    public DateTimeOffset CreatedAt { get; set; }
    public DateTimeOffset LastUsedAt { get; set; }

    public string? LastDeviceInfo { get; set; }
    public string? LastIpAddress { get; set; }

    public SessionState State { get; set; }
}

[JsonConverter(typeof(JsonStringEnumConverter))]
public enum SessionState
{
    Active = 0,
    LoggedOut = 1,
    ReuseDetected = 2,
    Terminated = 3
}