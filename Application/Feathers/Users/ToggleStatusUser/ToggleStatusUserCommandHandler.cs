namespace Application.Feathers.Users.ToggleStatusUser;

public class ToggleStatusUserCommandHandler(IUnitOfWork unitOfWork) : IRequestHandler<ToggleStatusUserCommand, Result>
{
    private readonly IUnitOfWork _unitOfWork = unitOfWork;

    public async Task<Result> Handle(ToggleStatusUserCommand request, CancellationToken cancellationToken = default)
    {
        if (await _unitOfWork.Users.FindByIdAsync(request.UserId, cancellationToken) is not { } user)
            return Result.Failure(UserErrors.NotFound);

        user.IsDisabled = !user.IsDisabled;

        var result = await _unitOfWork.Users.UpdateAsync(user);

        return result;
    }
}
