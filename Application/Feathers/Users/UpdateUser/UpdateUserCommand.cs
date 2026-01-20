namespace Application.Feathers.Users.UpdateUser;

public record UpdateUserCommand(string UserId, UpdateUserRequest Request) : IRequest<Result>;

