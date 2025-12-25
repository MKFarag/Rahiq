using Application.Interfaces;

namespace Application.Feathers.Auth.RevokeRefreshToken;

public class RevokeRefreshTokenCommandHandler(IJwtProvider jwtProvider, IUnitOfWork unitOfWork) : IRequestHandler<RevokeRefreshTokenCommand, Result>
{
    private readonly IJwtProvider _jwtProvider = jwtProvider;
    private readonly IUnitOfWork _unitOfWork = unitOfWork;

    public async Task<Result> Handle(RevokeRefreshTokenCommand request, CancellationToken cancellationToken)
    {
        if (_jwtProvider.ValidateToken(request.Token) is not { } userId)
            return Result.Failure(UserErrors.InvalidJwtToken);

        if (await _unitOfWork.Users.FindByIdAsync(userId, cancellationToken) is not { } user)
            return Result.Failure(UserErrors.InvalidJwtToken);

        var revokeResult = await _unitOfWork.Users.RevokeRefreshTokenAsync(user, request.RefreshToken, cancellationToken);

        if (revokeResult.IsFailure)
            return Result.Failure<AuthResponse>(revokeResult.Error);

        return Result.Success();
    }
}
