namespace Application.Feathers.Users.GetAllUsers;

public class GetAllUsersQueryHandler(IUnitOfWork unitOfWork) : IRequestHandler<GetAllUsersQuery, IEnumerable<UserResponse>>
{
    private readonly IUnitOfWork _unitOfWork = unitOfWork;

    public async Task<IEnumerable<UserResponse>> Handle(GetAllUsersQuery request, CancellationToken cancellationToken = default)
        => await _unitOfWork.Users.GetAllProjectionWithRolesAsync<UserResponse>(false, cancellationToken);
}
