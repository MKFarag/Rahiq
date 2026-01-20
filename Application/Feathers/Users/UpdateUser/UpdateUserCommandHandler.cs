namespace Application.Feathers.Users.UpdateUser;

public class UpdateUserCommandHandler(IUnitOfWork unitOfWork) : IRequestHandler<UpdateUserCommand, Result>
{
    private readonly IUnitOfWork _unitOfWork = unitOfWork;

    public async Task<Result> Handle(UpdateUserCommand command, CancellationToken cancellationToken = default)
    {
        if (await _unitOfWork.Users.EmailExistsAsync(command.Request.Email, command.UserId, cancellationToken))
            return Result.Failure(UserErrors.DuplicatedEmail);

        var allowedRoles = await _unitOfWork.Roles.GetAllNamesAsync(false, false, cancellationToken);

        if (command.Request.Roles.Except(allowedRoles).Any())
            return Result.Failure(UserErrors.InvalidRoles);

        if (await _unitOfWork.Users.FindByIdAsync(command.UserId, cancellationToken) is not { } user)
            return Result.Failure(UserErrors.NotFound);

        user = command.Request.Adapt(user);

        var result = await _unitOfWork.Users.UpdateAsync(user);

        if (result.IsFailure)
            return result;

        await _unitOfWork.Users.DeleteAllRolesAsync(user);

        await _unitOfWork.Users.AddToRolesAsync(user, command.Request.Roles);

        return Result.Success();
    }
}
