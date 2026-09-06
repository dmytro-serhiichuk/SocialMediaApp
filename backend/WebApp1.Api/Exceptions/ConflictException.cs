namespace WebApp1.Api.Exceptions;

public class ConflictException(IDictionary<string, string[]> errors) : AppException("One or more conflicts occurred.")
{
    public IDictionary<string, string[]> Errors { get; } = errors;
}