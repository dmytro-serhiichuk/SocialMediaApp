namespace WebApp1.Api.DTOs;

public class SettingsDataResponse
{
    public int Id { get; set; }
    public string UserName { get; set; } = string.Empty;
    public string UserTag { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public List<SessionResponse> Sessions { get; set; } = null!;
    public bool HasPassword { get; set; }
}