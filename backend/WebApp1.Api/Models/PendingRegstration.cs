namespace WebApp1.Api.Models;

public class PendingRegistration
{
    public Guid Id { get; set; }
    public string Email { get; set; } = string.Empty;
    public string UserName { get; set; } = string.Empty;
    public string UserTag { get; set; } = string.Empty;
    public string PasswordHash { get; set; } = string.Empty;
    public string TokenHash { get; set; } = string.Empty;
    public DateTimeOffset ExpiresAt { get; set; }
}