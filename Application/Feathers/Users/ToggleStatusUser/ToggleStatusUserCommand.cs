namespace Application.Feathers.Users.ToggleStatusUser;

public record ToggleStatusUserCommand(string UserId) : IRequest<Result>;

