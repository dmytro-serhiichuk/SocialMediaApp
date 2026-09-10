using WebApp1.Api.Models;

namespace WebApp1.Api.Utils;

public abstract record ExternalLoginResult
{
    public sealed record Success(User User) : ExternalLoginResult;
    public sealed record EmailAlreadyTaken(string Email) : ExternalLoginResult;
}