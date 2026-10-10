using SocialMediaApp.Api.Models;

namespace SocialMediaApp.Api.Utils;

public abstract record ConnectExternalLoginResult
{
    public sealed record Success : ConnectExternalLoginResult;
    public sealed record AlreadyLinkedToThisAccount : ConnectExternalLoginResult;
    public sealed record AlreadyLinkedToAnotherAccount : ConnectExternalLoginResult;
}