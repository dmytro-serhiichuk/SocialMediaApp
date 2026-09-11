using WebApp1.Api.Models;

namespace WebApp1.Api.Utils;

public abstract record ConnectExternalLoginResult
{
    public sealed record Success : ConnectExternalLoginResult;
    public sealed record AlreadyLinkedToThisAccount : ConnectExternalLoginResult;
    public sealed record AlreadyLinkedToAnotherAccount : ConnectExternalLoginResult;
}