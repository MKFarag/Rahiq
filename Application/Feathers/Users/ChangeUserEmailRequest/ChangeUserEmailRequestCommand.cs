namespace Application.Feathers.Users.ChangeUserEmailRequest;

public record ChangeUserEmailRequestCommand(string UserId, string NewEmail) : IRequest<Result>;

