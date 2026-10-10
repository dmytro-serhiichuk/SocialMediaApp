namespace SocialMediaApp.Api.Exceptions;

public class EmailAlreadyExistsException() : AppException("The provided email is already in use.")
{
}