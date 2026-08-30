namespace WebApp1.Api.DTOs;

public class SettingsDataResponse
{
    public int Id { get; set; }
    public string UserName { get; set; } = null!;
    public string UserTag { get; set; } = null!;
    public string Email { get; set; } = null!;
}