namespace Application.Feathers.Roles.AddRole;

public class AddRoleCommandHandler(IUnitOfWork unitOfWork) : IRequestHandler<AddRoleCommand, Result<RoleDetailResponse>>
{
    private readonly IUnitOfWork _unitOfWork = unitOfWork;

    public async Task<Result<RoleDetailResponse>> Handle(AddRoleCommand command, CancellationToken cancellationToken = default)
    {
        if (await _unitOfWork.Roles.NameExistsAsync(command.Request.Name, cancellationToken))
            return Result.Failure<RoleDetailResponse>(RoleErrors.DuplicatedName);

        var allowedPermissions = Permissions.GetAll();

        if (command.Request.Permissions.Except(allowedPermissions).Any())
            return Result.Failure<RoleDetailResponse>(RoleErrors.InvalidPermissions);

        var role = command.Request.Adapt<Role>();

        var result = await _unitOfWork.Roles.CreateAsync(role);

        if (result.IsFailure)
            return Result.Failure<RoleDetailResponse>(result.Error);

        await _unitOfWork.Roles.AddClaimsAsync(role.Id, Permissions.Type, command.Request.Permissions, cancellationToken);

        var response = new RoleDetailResponse(role.Id, role.Name, role.IsDisabled, command.Request.Permissions);

        return Result.Success(response);
    }
}
