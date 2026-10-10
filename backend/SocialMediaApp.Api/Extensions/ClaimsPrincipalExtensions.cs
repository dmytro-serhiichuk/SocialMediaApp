using System.Security.Claims;

namespace SocialMediaApp.Api.Extensions;

public static class ClaimsPrincipalExtensions
{
    public static int GetUserId(this ClaimsPrincipal user)
    {
        var value = user.FindFirstValue(ClaimTypes.NameIdentifier) 
            ?? throw new InvalidOperationException("User ID claim is missing.");
        
        if (!int.TryParse(value, out int id))
            throw new InvalidOperationException("User ID claim type is incorrect.");

        return id;
    }

    public static int GetSessionId(this ClaimsPrincipal user)
    {
        var value = user.FindFirstValue("sid") 
            ?? throw new InvalidOperationException("Session ID claim is missing.");
        
        if (!int.TryParse(value, out int sessionId))
            throw new InvalidOperationException("Session ID claim type is incorrect.");

        return sessionId;
    }
}