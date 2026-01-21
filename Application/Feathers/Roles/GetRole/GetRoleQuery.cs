namespace Application.Feathers.Roles.GetRole;

public record GetRoleQuery(string Id) : IRequest<Result<RoleDetailResponse>>;

