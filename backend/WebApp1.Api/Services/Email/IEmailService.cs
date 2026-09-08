namespace WebApp1.Api.Services.Email;

public interface IEmailService
{
    Task SendAsync(
        string recipientEmail, 
        string subject, 
        string htmlBody, 
        CancellationToken cancellationToken = default
    );

    Task SendEmailConfirmationMessageAsync(
        string email, 
        string token, 
        CancellationToken cancellationToken = default
    );

    Task SendRegistrationConfirmationMessageAsync(
        string email, 
        string token, 
        CancellationToken cancellationToken = default
    );

    Task SendPasswordResetMessageAsync(
        string email, 
        string token, 
        CancellationToken cancellationToken = default
    );
}