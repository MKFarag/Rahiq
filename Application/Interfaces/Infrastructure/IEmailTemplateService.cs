namespace Application.Interfaces.Infrastructure;

public interface IEmailTemplateService
{
    void SendConfirmationLink(User user, string code, int expiryTimeInHours = 24);
    void SendResetPassword(User user, string code, int expiryTimeInHours = 24);
    void SendChangeEmailNotification(User user, string oldEmail, DateTime changeDate);
}
