namespace Application.Feathers.Auth.ConfirmEmail;

public record ConfirmEmailCommand(string UserId, string Token) : IRequest<Result>;
