using SocialMediaApp.Api.Extensions;

namespace SocialMediaApp.Api.Services;

public class CurrentUserService
{
    private readonly IHttpContextAccessor _httpContextAccessor;

    public CurrentUserService(IHttpContextAccessor httpContextAccessor)
    {
        _httpContextAccessor = httpContextAccessor;
    }

    public int? UserId
    {
        get
        {
            var user = _httpContextAccessor.HttpContext?.User;

            if (user?.Identity?.IsAuthenticated != true)
                return null;

            return user.GetUserId();
        }
    }
}