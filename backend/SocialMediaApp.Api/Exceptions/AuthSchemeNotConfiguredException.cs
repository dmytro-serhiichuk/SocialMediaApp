namespace SocialMediaApp.Api.Exceptions;

public class AuthSchemeNotConfiguredException(string scheme) : AppException($"{scheme} sign-in is not configured.")
{
}