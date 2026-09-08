namespace WebApp1.Api.Exceptions;

public class ConfirmationTokenExpiredException() : AppException("The provided token is expired.")
{
}