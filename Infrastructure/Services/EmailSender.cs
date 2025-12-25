using Application.Interfaces;
using MailKit.Net.Smtp;
using MailKit.Security;
using MimeKit;

namespace Infrastructure.Services;

public class EmailSender(IOptions<MailSettings> mailSettings) : IEmailSender
{
    private readonly MailSettings _settings = mailSettings.Value;

    public async Task SendAsync(string email, string subject, string body)
    {
        var message = new MimeMessage
        {
            Sender = MailboxAddress.Parse(_settings.Mail),
            Subject = subject
        };

        message.To.Add(MailboxAddress.Parse(email));

        var builder = new BodyBuilder
        {
            HtmlBody = body
        };

        message.Body = builder.ToMessageBody();

        using var smtp = new SmtpClient();

        await smtp.ConnectAsync(_settings.Host, _settings.Port, SecureSocketOptions.StartTls);
        await smtp.AuthenticateAsync(_settings.Mail, _settings.Password);
        await smtp.SendAsync(message);
        await smtp.DisconnectAsync(true);
    }
}