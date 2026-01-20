namespace Application.Feathers.Users.GetUser;

public class GetUserQueryHandler(IUnitOfWork unitOfWork) : IRequestHandler<GetUserQuery, Result<UserResponse>>
{
    private readonly IUnitOfWork _unitOfWork = unitOfWork;

    public async Task<Result<UserResponse>> Handle(GetUserQuery request, CancellationToken cancellationToken = default)
    {
        if (await _unitOfWork.Users.FindByIdAsync(request.UserId, cancellationToken) is not { } user)
            return Result.Failure<UserResponse>(UserErrors.NotFound);

        var roles = await _unitOfWork.Users.GetRolesAsync(user);

        var response = (user, roles).Adapt<UserResponse>();

        return Result.Success(response);
    }
}
