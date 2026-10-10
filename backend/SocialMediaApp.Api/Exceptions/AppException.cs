namespace SocialMediaApp.Api.Exceptions;

public abstract class AppException(string message) : Exception(message)
{
}