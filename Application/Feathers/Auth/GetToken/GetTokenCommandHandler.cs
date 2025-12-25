namespace Application.Feathers.Auth.GetToken;

public class GetTokenCommandHandler(ISignInService signInService, IJwtProvider jwtProvider, IUnitOfWork unitOfWork) : IRequestHandler<GetTokenCommand, Result<AuthResponse>>
{
    private readonly ISignInService _signInService = signInService;
    private readonly IJwtProvider _jwtProvider = jwtProvider;
    private readonly IUnitOfWork _unitOfWork = unitOfWork;

    public async Task<Result<AuthResponse>> Handle(GetTokenCommand request, CancellationToken cancellationToken)
    {
        var result = await _signInService.PasswordSignInAsync(request.Identifier, request.Password, false);

        if (result.IsFailure)
            return Result.Failure<AuthResponse>(result.Error);

        var user = result.Value;

        if (user.IsDisabled)
            return Result.Failure<AuthResponse>(UserErrors.DisabledUser);

        var (userRoles, userPermissions) = await _unitOfWork.Users.GetRolesAndPermissionsAsync(user, cancellationToken);

        var (token, expiresIn) = _jwtProvider.GenerateToken(user, userRoles, userPermissions);

        var refreshToken = RefreshTokenHandler.GenerateNewToken();
        var refreshTokenExpiration = DateTime.UtcNow.AddDays(RefreshTokenHandler.ExpiryDays);

        await _unitOfWork.Users.AddRefreshTokenAsync(user, new RefreshToken
        {
            Token = refreshToken,
            ExpiresOn = refreshTokenExpiration
        }, cancellationToken);

        var response = new AuthResponse(user.Id, user.Email, user.FirstName, token, expiresIn, refreshToken, refreshTokenExpiration);

        return Result.Success(response);
    }
}
