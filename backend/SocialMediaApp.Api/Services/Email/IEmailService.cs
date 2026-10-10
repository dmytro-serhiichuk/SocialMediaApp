using SocialMediaApp.Api.Models;

namespace SocialMediaApp.Api.Services.Email;

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

    Task SendPasswordCreateMessageAsync(
        string email, 
        string token, 
        CancellationToken cancellationToken = default
    );

    Task SendNewLoginNotificationMessageAsync(
        string email, 
        Session session,
        string token, 
        CancellationToken cancellationToken = default
    );
}