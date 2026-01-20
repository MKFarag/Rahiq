namespace Application.Feathers.Users.ChangeUserPassword;

public class ChangeUserPasswordCommandHandler(IUnitOfWork unitOfWork) : IRequestHandler<ChangeUserPasswordCommand, Result>
{
    private readonly IUnitOfWork _unitOfWork = unitOfWork;

    public async Task<Result> Handle(ChangeUserPasswordCommand command, CancellationToken cancellationToken = default)
    {
        if (await _unitOfWork.Users.FindByIdAsync(command.UserId, cancellationToken) is not { } user)
            return Result.Failure(UserErrors.NotFound);

        var result = await _unitOfWork.Users.ChangePasswordAsync(user, command.Request.CurrentPassword, command.Request.NewPassword);

        return result;
    }
}
