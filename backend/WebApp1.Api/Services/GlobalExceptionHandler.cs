using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using WebApp1.Api.Exceptions;

namespace WebApp1.Api.Services;

public class GlobalExceptionHandler(ILogger<GlobalExceptionHandler> logger) : IExceptionHandler
{
    private readonly ILogger<GlobalExceptionHandler> logger = logger;

    public async ValueTask<bool> TryHandleAsync(HttpContext httpContext, Exception exception, CancellationToken cancellationToken)
    {
        if (exception is AppException)
        {
            logger.LogError(exception, exception.Message);

            var status = GetStatusCode(exception);
            httpContext.Response.StatusCode = status;

            if (exception is ConflictException conflictException)
            {
                var problemDetails = new ValidationProblemDetails(conflictException.Errors)
                {
                    Status = status,
                    Title = exception.Message,
                };

                await httpContext.Response.WriteAsJsonAsync(problemDetails, cancellationToken);
            }
            else
            {
                var problemDetails = new ProblemDetails
                {
                    Status = status,
                    Title = exception.Message,
                };

                await httpContext.Response.WriteAsJsonAsync(problemDetails, cancellationToken);
            }
        } 
        else
        {
            logger.LogError(exception, "Unhandled exception occurred.");

            var problemDetails = new ProblemDetails
            {
                Status = StatusCodes.Status500InternalServerError,
                Title = "An unexpected error occurred while processing the request.",
            };

            httpContext.Response.StatusCode =
                StatusCodes.Status500InternalServerError;

            await httpContext.Response.WriteAsJsonAsync(
                problemDetails,
                cancellationToken);
        }

        return true;
    }

    private static int GetStatusCode(Exception exception)
    {
        return exception switch
        {
            UserTagAlreadyTakenException => StatusCodes.Status409Conflict,
            InvalidConfirmationTokenException => StatusCodes.Status400BadRequest,
            ConfirmationTokenExpiredException => StatusCodes.Status400BadRequest,
            EmailAlreadyExistsException => StatusCodes.Status409Conflict,
            IncorrectPasswordException => StatusCodes.Status400BadRequest,
            ConflictException => StatusCodes.Status409Conflict,
            AccountRecoveryInProgressException => StatusCodes.Status403Forbidden,
            AccountRecoveryNotAvailableException => StatusCodes.Status409Conflict,
            ExternalLoginAlreadyExistsException => StatusCodes.Status403Forbidden,
            InvalidSessionStateException => StatusCodes.Status409Conflict,
            InvalidPostImagesException => StatusCodes.Status400BadRequest,
            _ => StatusCodes.Status500InternalServerError
        };
    }
}