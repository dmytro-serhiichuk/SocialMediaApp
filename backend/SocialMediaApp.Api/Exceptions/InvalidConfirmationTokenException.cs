namespace SocialMediaApp.Api.Exceptions;

public class InvalidConfirmationTokenException() : AppException("The provided token is invalid.")
{
}