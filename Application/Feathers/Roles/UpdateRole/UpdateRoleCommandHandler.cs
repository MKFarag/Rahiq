namespace Application.Feathers.Roles.UpdateRole;

public class UpdateRoleCommandHandler(IUnitOfWork unitOfWork) : IRequestHandler<UpdateRoleCommand, Result>
{
    private readonly IUnitOfWork _unitOfWork = unitOfWork;

    public async Task<Result> Handle(UpdateRoleCommand command, CancellationToken cancellationToken = default)
    {
        if (await _unitOfWork.Roles.GetAsync(command.Id, cancellationToken) is not { } role)
            return Result.Failure(RoleErrors.NotFound);

        if (await _unitOfWork.Roles.NameExistsAsync(command.Request.Name, role.Id, cancellationToken))
            return Result.Failure(RoleErrors.DuplicatedName);

        var allowedPermissions = Permissions.GetAll();

        if (command.Request.Permissions.Except(allowedPermissions).Any())
            return Result.Failure(RoleErrors.InvalidPermissions);

        role.Name = command.Request.Name;

        var result = await _unitOfWork.Roles.UpdateAsync(role);

        if (result.IsFailure)
            return result;

        var currentPermissions = await _unitOfWork.Roles.GetClaimsAsync(role.Id, cancellationToken);

        var newPermissions = command.Request.Permissions.Except(currentPermissions);

        var removedPermissions = currentPermissions.Except(command.Request.Permissions);

        await _unitOfWork.Roles.DeleteClaimsAsync(role.Id, removedPermissions, cancellationToken);

        await _unitOfWork.Roles.AddClaimsAsync(role.Id, Permissions.Type, newPermissions, cancellationToken);

        return Result.Success();
    }
}
