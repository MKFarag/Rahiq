namespace Application.Feathers.Roles.ToggleRoleStatus;

public class ToggleRoleStatusCommandHandler(IUnitOfWork unitOfWork) : IRequestHandler<ToggleRoleStatusCommand, Result>
{
    private readonly IUnitOfWork _unitOfWork = unitOfWork;

    public async Task<Result> Handle(ToggleRoleStatusCommand request, CancellationToken cancellationToken = default)
    {
        if (await _unitOfWork.Roles.GetAsync(request.Id, cancellationToken) is not { } role)
            return Result.Failure(RoleErrors.NotFound);

        role.IsDisabled = !role.IsDisabled;

        await _unitOfWork.Roles.UpdateAsync(role);

        return Result.Success();
    }
}
