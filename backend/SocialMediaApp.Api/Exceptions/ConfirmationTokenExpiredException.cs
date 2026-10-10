namespace SocialMediaApp.Api.Exceptions;

public class ConfirmationTokenExpiredException() : AppException("The provided token is expired.")
{
}