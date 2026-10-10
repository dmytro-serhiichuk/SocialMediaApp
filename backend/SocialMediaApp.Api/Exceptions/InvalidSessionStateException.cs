namespace SocialMediaApp.Api.Exceptions;

public class InvalidSessionStateException() : AppException("The operation cannot be performed due to invalid session state.")
{
}