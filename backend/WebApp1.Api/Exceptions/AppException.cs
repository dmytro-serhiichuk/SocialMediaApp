namespace WebApp1.Api.Exceptions;

public abstract class AppException(string message) : Exception(message)
{
}