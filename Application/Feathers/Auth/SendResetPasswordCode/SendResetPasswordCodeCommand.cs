namespace Application.Feathers.Auth.SendResetPasswordCode;

public record SendResetPasswordCodeCommand(string Email) : IRequest<Result>;
