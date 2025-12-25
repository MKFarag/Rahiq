namespace Application.Feathers.Auth.ResendConfirmationEmail;

public record ResendConfirmationEmailCommand(string Email) : IRequest<Result>;
