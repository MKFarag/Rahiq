using System.Security.Cryptography;

namespace Application.Feathers.Auth.GetRefreshToken;

public class GetRefreshTokenCommandHandler(IJwtProvider jwtProvider, IUnitOfWork unitOfWork) : IRequestHandler<GetRefreshTokenCommand, Result<AuthResponse>>
{
    private readonly IJwtProvider _jwtProvider = jwtProvider;
    private readonly IUnitOfWork _unitOfWork = unitOfWork;

    private readonly int _refreshTokenExpiryDays = 14;

    public async Task<Result<AuthResponse>> Handle(GetRefreshTokenCommand request, CancellationToken cancellationToken)
    {
        if (_jwtProvider.ValidateToken(request.Token) is not { } userId)
            return Result.Failure<AuthResponse>(UserErrors.InvalidJwtToken);

        if (await _unitOfWork.Users.FindByIdAsync(userId, cancellationToken) is not { } user)
            return Result.Failure<AuthResponse>(UserErrors.InvalidJwtToken);

        if (user.IsDisabled)
            return Result.Failure<AuthResponse>(UserErrors.DisabledUser);

        var revokeResult = await _unitOfWork.Users.RevokeRefreshTokenAsync(user, request.RefreshToken, cancellationToken);

        if (revokeResult.IsFailure)
            return Result.Failure<AuthResponse>(revokeResult.Error);

        var (userRoles, userPermissions) = await _unitOfWork.Users.GetRolesAndPermissionsAsync(user, cancellationToken);

        var (newToken, expiresIn) = _jwtProvider.GenerateToken(user, userRoles, userPermissions);

        var newRefreshToken = Convert.ToBase64String(RandomNumberGenerator.GetBytes(64));
        var refreshTokenExpiration = DateTime.UtcNow.AddDays(_refreshTokenExpiryDays);

        await _unitOfWork.Users.AddRefreshTokenAsync(user, new RefreshToken
        {
            Token = newRefreshToken,
            ExpiresOn = refreshTokenExpiration
        }, cancellationToken);

        var response = new AuthResponse(user.Id, user.Email, user.FirstName, newToken, expiresIn, newRefreshToken, refreshTokenExpiration);

        return Result.Success(response);
    }
}
