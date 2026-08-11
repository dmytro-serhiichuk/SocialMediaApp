namespace WebApp1.Api.Models;

public class GetUserResponse
{
    public string UserName { get; set; } = string.Empty;
    public string UserTag { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;

    public static GetUserResponse CreateFromUser(User user)
    {
        return new GetUserResponse
        {
            UserName = user.UserName,
            UserTag = user.UserTag,
            Email = user.Email
        };
    }
}