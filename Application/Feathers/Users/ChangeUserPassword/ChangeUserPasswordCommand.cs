namespace Application.Feathers.Users.ChangeUserPassword;

public record ChangeUserPasswordCommand(string UserId, ChangePasswordRequest Request) : IRequest<Result>;

