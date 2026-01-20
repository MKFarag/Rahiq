namespace Application.Feathers.Users.AddUser;

public record AddUserCommand(CreateUserRequest Request) : IRequest<Result<UserResponse>>;

