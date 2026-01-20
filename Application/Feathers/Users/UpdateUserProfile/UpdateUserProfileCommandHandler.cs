namespace Application.Feathers.Users.UpdateUserProfile;

public class UpdateUserProfileCommandHandler(IUnitOfWork unitOfWork) : IRequestHandler<UpdateUserProfileCommand, Result>
{
    private readonly IUnitOfWork _unitOfWork = unitOfWork;

    public async Task<Result> Handle(UpdateUserProfileCommand command, CancellationToken cancellationToken = default)
    {
        if (await _unitOfWork.Users.FindByIdAsync(command.UserId, cancellationToken) is not { } user)
            return Result.Failure(UserErrors.NotFound);

        user = command.Request.Adapt(user);

        await _unitOfWork.Users.UpdateAsync(user);

        return Result.Success();
    }
}
