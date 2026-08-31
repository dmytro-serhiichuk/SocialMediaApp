namespace WebApp1.Api.Exceptions;

public class UserTagAlreadyTakenException() : AppException("The specified tag is already taken.")
{
}