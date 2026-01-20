namespace Application.Feathers.Users.GetAllUsers;

public record GetAllUsersQuery() : IRequest<IEnumerable<UserResponse>>;

