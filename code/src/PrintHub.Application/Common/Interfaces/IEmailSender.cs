namespace PrintHub.Application.Common.Interfaces;

/// <summary>Sends transactional email. Implemented in Infrastructure (SMTP via MailKit);
/// a no-op when SMTP is not configured so callers never need to guard.</summary>
public interface IEmailSender
{
    bool IsEnabled { get; }
    Task SendAsync(string toEmail, string? toName, string subject, string htmlBody, CancellationToken ct = default);
}
