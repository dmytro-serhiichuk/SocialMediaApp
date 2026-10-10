using SocialMediaApp.Api.Models;

namespace SocialMediaApp.Api.Utils;

public abstract record ExternalLoginResult
{
    public sealed record Success(User User) : ExternalLoginResult;
    public sealed record EmailAlreadyTaken(string Email) : ExternalLoginResult;
}