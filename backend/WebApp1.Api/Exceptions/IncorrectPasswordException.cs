namespace WebApp1.Api.Exceptions;

public class IncorrectPasswordException() : AppException("The provided password is incorrect.")
{
}