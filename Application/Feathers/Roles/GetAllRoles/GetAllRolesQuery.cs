namespace Application.Feathers.Roles.GetAllRoles;

public record GetAllRolesQuery(bool IncludeDisabled = false) : IRequest<IEnumerable<RoleResponse>>;

