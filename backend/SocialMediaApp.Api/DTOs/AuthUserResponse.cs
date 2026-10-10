using SocialMediaApp.Api.Models;

namespace SocialMediaApp.Api.DTOs;

public class AuthUserResponse
{
    public int Id { get; set; }
    public string UserName { get; set; } = string.Empty;
    public string UserTag { get; set; } = string.Empty;

    public static AuthUserResponse CreateFromUser(User user)
    {
        return new AuthUserResponse
        {
            Id = user.Id,
            UserName = user.UserName,
            UserTag = user.UserTag
        };
    }
}