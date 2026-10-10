namespace SocialMediaApp.Api.Configuration;

public class ConfiramtionTokenOptions
{
    public const string SectionName = "ConfirmationToken";

    public int EmailChangeLifetimeMinutes { get; set; }
    public int EmailConfirmationLifetimeHours { get; set; }
    public int PasswordResetLifetimeMinutes { get; set; }
    public int NewLoginTokenLifetimeMinutes { get; set; }
}