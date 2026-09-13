using WebApp1.Api.Models;

namespace WebApp1.Api.DTOs;

public class UserResponse
{
    public int Id { get; set; }
    public string UserName { get; set; } = string.Empty;
    public string UserTag { get; set; } = string.Empty;
    public bool IsFollowedByMe { get; set; }
    public bool IsFollowingMe { get; set; }
}