namespace Application.Feathers.Roles.UpdateRole;

public record UpdateRoleCommand(string Id, RoleRequest Request) : IRequest<Result>;

