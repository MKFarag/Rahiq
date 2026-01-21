namespace Application.Feathers.Roles.AddRole;

public record AddRoleCommand(RoleRequest Request) : IRequest<Result<RoleDetailResponse>>;

