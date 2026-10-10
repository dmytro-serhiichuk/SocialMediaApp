namespace SocialMediaApp.Api.Exceptions;

public class IncorrectPasswordException() : AppException("The provided password is incorrect.")
{
}