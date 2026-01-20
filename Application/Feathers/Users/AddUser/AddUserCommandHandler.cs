namespace Application.Feathers.Users.AddUser;

public class AddUserCommandHandler(IUnitOfWork unitOfWork) : IRequestHandler<AddUserCommand, Result<UserResponse>>
{
    private readonly IUnitOfWork _unitOfWork = unitOfWork;

    public async Task<Result<UserResponse>> Handle(AddUserCommand command, CancellationToken cancellationToken = default)
    {
        if (await _unitOfWork.Users.EmailExistsAsync(command.Request.Email, cancellationToken))
            return Result.Failure<UserResponse>(UserErrors.DuplicatedEmail);

        var allowedRoles = await _unitOfWork.Roles.GetAllNamesAsync(false, false, cancellationToken);

        if (command.Request.Roles.Except(allowedRoles).Any())
            return Result.Failure<UserResponse>(UserErrors.InvalidRoles);

        var user = command.Request.Adapt<User>();

        var result = await _unitOfWork.Users.CreateAsync(user, command.Request.Password, true);

        if (result.IsFailure)
            return Result.Failure<UserResponse>(result.Error);

        await _unitOfWork.Users.AddToRolesAsync(user, command.Request.Roles);

        var response = (user, command.Request.Roles).Adapt<UserResponse>();

        return Result.Success(response);
    }
}
