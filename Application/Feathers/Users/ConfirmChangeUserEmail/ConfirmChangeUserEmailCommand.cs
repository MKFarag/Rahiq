namespace Application.Feathers.Users.ConfirmChangeUserEmail;

public record ConfirmChangeUserEmailCommand(string UserId, ConfirmChangeEmailRequest Request) : IRequest<Result>;

