namespace Application.Feathers.Roles.ToggleRoleStatus;

public record ToggleRoleStatusCommand(string Id) : IRequest<Result>;

