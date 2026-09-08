
using MailKit.Net.Smtp;
using MailKit.Security;
using Microsoft.Extensions.Options;
using MimeKit;
using WebApp1.Api.Configuration;

namespace WebApp1.Api.Services.Email;

public class EmailService(
    IOptions<EmailOptions> emailOptions, 
    IOptions<FrontendOptions> frontendOptions) : IEmailService
{
    private readonly EmailOptions emailOptions = emailOptions.Value;
    private readonly FrontendOptions frontendOptions = frontendOptions.Value;

    public async Task SendAsync(string recipientEmail, string subject, string htmlBody, CancellationToken cancellationToken = default)
    {
        var message = new MimeMessage();

        message.From.Add(new MailboxAddress(emailOptions.SenderName, emailOptions.SenderEmail));
        message.To.Add(MailboxAddress.Parse(recipientEmail));
        message.Subject = subject;
        message.Body = new BodyBuilder { HtmlBody = htmlBody }.ToMessageBody();

        using var client = new SmtpClient();

        await client.ConnectAsync(emailOptions.Host, emailOptions.Port, SecureSocketOptions.StartTls, cancellationToken);
        await client.AuthenticateAsync(emailOptions.Username, emailOptions.Password, cancellationToken);
        await client.SendAsync(message, cancellationToken);
        await client.DisconnectAsync(true, cancellationToken);
    }

    public async Task SendEmailConfirmationMessageAsync(string email, string token, CancellationToken cancellationToken = default)
    {
        var confirmationUrl = $"{frontendOptions.Url}/settings/email-change/confirm#token={token}";

        await SendAsync(
            email, 
            "Confirm your email change", 
            $"""
            <h1>Confirm your email change</h1>

            <p>
                Click the link below to confirm your new email address.
            </p>

            <p>
                <a href="{confirmationUrl}">
                    Confirm email change
                </a>
            </p>
            """,
            cancellationToken
        );
    }

    public async Task SendPasswordResetMessageAsync(string email, string token, CancellationToken cancellationToken = default)
    {
        var confirmationUrl = $"{frontendOptions.Url}/password-reset/complete#token={token}";

        await SendAsync(
            email, 
            "Password reset request", 
            $"""
            <h1>Password reset request</h1>

            <p>
                You have requested a password reset for your profile. Click the link below to confirm and continue you request.
            </p>

            <p>
                <a href="{confirmationUrl}">
                    Confirm email
                </a>
            </p>
            """,
            cancellationToken
        );
    }

    public async Task SendRegistrationConfirmationMessageAsync(string email, string token, CancellationToken cancellationToken = default)
    {
        var confirmationUrl = $"{frontendOptions.Url}/register/confirm#token={token}";

        await SendAsync(
            email, 
            "Confirm your registration", 
            $"""
            <h1>Confirm your registration</h1>

            <p>
                Click the link below to confirm your email address.
            </p>

            <p>
                <a href="{confirmationUrl}">
                    Confirm email
                </a>
            </p>
            """,
            cancellationToken
        );
    }
}