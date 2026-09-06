namespace WebApp1.Api.Configuration;

public class ConfiramtionTokenOptions
{
    public const string SectionName = "ConfirmationToken";

    public int EmailChangeLifetimeMinutes { get; set; }
    public int EmailConfirmationLifetimeHours { get; set; }
}