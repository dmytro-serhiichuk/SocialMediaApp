namespace WebApp1.Api.Models;

public class ExternalLogin
{
    public int Id { get; set; }
    public int UserId { get; set; }
    public User User { get; set; } = null!;
    public ExternalLoginProvider Provider { get; set; }
    public string ProviderUserId { get; set; } = null!;
}

public enum ExternalLoginProvider
{
    Google = 0
}