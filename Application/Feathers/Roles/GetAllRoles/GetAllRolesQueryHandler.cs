namespace Application.Feathers.Roles.GetAllRoles;

public class GetAllRolesQueryHandler(IUnitOfWork unitOfWork) : IRequestHandler<GetAllRolesQuery, IEnumerable<RoleResponse>>
{
    private readonly IUnitOfWork _unitOfWork = unitOfWork;

    public async Task<IEnumerable<RoleResponse>> Handle(GetAllRolesQuery request, CancellationToken cancellationToken = default)
    {
        var roles = await _unitOfWork.Roles.GetAllAsync(false, request.IncludeDisabled, cancellationToken);

        return roles.Any()
            ? roles.Adapt<IEnumerable<RoleResponse>>()
            : [];
    }
}
