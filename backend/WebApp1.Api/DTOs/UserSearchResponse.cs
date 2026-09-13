using WebApp1.Api.Models;

namespace WebApp1.Api.DTOs;

public class UserSearchResponse
{
    public int Id { get; set; }
    public string UserName { get; set; } = string.Empty;
    public string UserTag { get; set; } = string.Empty;

    public static UserSearchResponse CreateFromUser(User user)
    {
        return new UserSearchResponse
        {
            Id = user.Id,
            UserName = user.UserName,
            UserTag = user.UserTag
        };
    }
}