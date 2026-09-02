namespace WebApp1.Api.Configuration;

public class EmailChangeOptions
{
    public const string SectionName = "EmailChange";

    public int TokenLifetimeMinutes { get; set; }
}