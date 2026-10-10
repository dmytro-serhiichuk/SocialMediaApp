using SocialMediaApp.Api.Models;

namespace SocialMediaApp.Api.Exceptions;

public class ExternalLoginAlreadyExistsException(ExternalLoginProvider provider) : 
    AppException($"An account with this email already exists. Sign in to your existing account and connect {provider} from Settings")
{
}