namespace Application.Interfaces.Infrastructure;

public interface INotificationService
{
    Task SendConfirmationLinkAsync(User user, string code, int expiryTimeInHours = 24);
    Task SendResetPasswordAsync(User user, string code, int expiryTimeInHours = 24);
    Task SendChangeEmailNotificationAsync(User user, string oldEmail, DateTime changeDate);
}
