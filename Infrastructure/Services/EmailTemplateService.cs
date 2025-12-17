using Microsoft.AspNetCore.Identity.UI.Services;

namespace Infrastructure.Services;

public class EmailTemplateService
    (IOptions<EmailTemplateOptions> templateData,
    IHttpContextAccessor httpContextAccessor,
    IEmailSender emailSender,
    IJobManager jobManager) : IEmailTemplateService
{
    private readonly IHttpContextAccessor _httpContextAccessor = httpContextAccessor;

    private readonly EmailTemplateOptions _templateData = templateData.Value;
    private readonly IEmailSender _emailSender = emailSender;
    private readonly IJobManager _jobManager = jobManager;

    /// <summary>
    /// Sends an email with a confirmation link to the user
    /// </summary>
    /// <param name="user">The user to send the confirmation link to</param>
    /// <param name="code">The confirmation code to include in the link</param>
    /// <param name="expiryTimeInHours">The expiry time in hours for the confirmation link</param>
    public void SendConfirmationLink(User user, string code, int expiryTimeInHours = 24)
    {
        var origin = _httpContextAccessor.HttpContext?.Request.Headers.Origin;

        var data = GetBaseEmailTemplateData(user.FirstName);
        data.Add(EmailTemplateOptions.Placeholders.ActionUrl,
            string.Format("{0}/auth/emailConfirmation?userId={1}&code={2}", origin, user.Id, code));
        data.Add(EmailTemplateOptions.Placeholders.ExpiryTime, expiryTimeInHours.ToString());

        EnqueueEmail(user.Email!, "Api.Builder email confirmation", EmailTemplateOptions.TemplatesNames.EmailConfirmationLink, data);
    }

    /// <summary>
    /// Sends an email with a password reset link to the user
    /// </summary>
    /// <param name="user">The user to send the reset password link to</param>
    /// <param name="code">The reset code to include in the link</param>
    /// <param name="expiryTimeInHours">The expiry time in hours for the reset link</param>
    public void SendResetPassword(User user, string code, int expiryTimeInHours = 24)
    {
        var origin = _httpContextAccessor.HttpContext?.Request.Headers.Origin;

        var data = GetBaseEmailTemplateData(user.FirstName);
        data.Add(EmailTemplateOptions.Placeholders.ActionUrl,
            string.Format("{0}/auth/forgetPassword?email={1}&code={2}", origin, user.Email, code));
        data.Add(EmailTemplateOptions.Placeholders.ExpiryTime, expiryTimeInHours.ToString());

        EnqueueEmail(user.Email!, "Api.Builder reset password", EmailTemplateOptions.TemplatesNames.ResetPassword, data);
    }

    /// <summary>
    /// Sends a notification email about email address change
    /// </summary>
    /// <param name="user">The user whose email was changed</param>
    /// <param name="oldEmail">The previous email address</param>
    /// <param name="changeDate">The date when the email was changed</param>
    public void SendChangeEmailNotification(User user, string oldEmail, DateTime changeDate)
    {
        var data = GetBaseEmailTemplateData(user.FirstName);
        data.Add(EmailTemplateOptions.Placeholders.NewEmail, user.Email!);
        data.Add(EmailTemplateOptions.Placeholders.ChangeDate, changeDate.ToString("f"));

        EnqueueEmail(oldEmail, "Api.Builder email change notification", EmailTemplateOptions.TemplatesNames.ChangeEmailNotification, data);
    }

    private void EnqueueEmail(string recipient, string subject, string template, Dictionary<string, string> data)
    {
        var emailBody = EmailBodyBuilder.GenerateEmailBody(template, data);
        _jobManager.Enqueue(() => _emailSender.SendEmailAsync(recipient, subject, emailBody));
    }

    private Dictionary<string, string> GetBaseEmailTemplateData(string userName)
        => new()
        {
            { EmailTemplateOptions.Placeholders.TitleName, _templateData.TitleName },
            { EmailTemplateOptions.Placeholders.TeamName, _templateData.TeamName },
            { EmailTemplateOptions.Placeholders.Address, _templateData.Address },
            { EmailTemplateOptions.Placeholders.City, _templateData.City },
            { EmailTemplateOptions.Placeholders.Country, _templateData.Country },
            { EmailTemplateOptions.Placeholders.SupportEmail, _templateData.SupportEmail},
            { EmailTemplateOptions.Placeholders.UserName, userName }
        };
}
