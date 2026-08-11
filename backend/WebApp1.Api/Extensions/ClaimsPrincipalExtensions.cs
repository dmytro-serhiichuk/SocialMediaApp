using System.Security.Claims;

namespace WebApp1.Api.Extensions;

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
}