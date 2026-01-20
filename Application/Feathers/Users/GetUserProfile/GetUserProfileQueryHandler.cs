namespace Application.Feathers.Users.GetUserProfile;

public class GetUserProfileQueryHandler(IUnitOfWork unitOfWork) : IRequestHandler<GetUserProfileQuery, Result>
{
    private readonly IUnitOfWork _unitOfWork = unitOfWork;

    public async Task<Result> Handle(GetUserProfileQuery request, CancellationToken cancellationToken = default)
    {
        if (await _unitOfWork.Users.FindByIdAsync(request.UserId, cancellationToken) is not { } user)
            return Result.Failure(UserErrors.NotFound);

        return Result.Success(user.Adapt<UserResponse>());
    }
}
