namespace Application.Feathers.Users.GetUser;

public record GetUserQuery(string UserId) : IRequest<Result<UserResponse>>;

