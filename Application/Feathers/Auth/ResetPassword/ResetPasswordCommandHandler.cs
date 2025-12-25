namespace Application.Feathers.Auth.ResetPassword;

public class ResetPasswordCommandHandler(IUnitOfWork unitOfWork, IUrlEncoder urlEncoder) : IRequestHandler<ResetPasswordCommand, Result>
{
    private readonly IUnitOfWork _unitOfWork = unitOfWork;
    private readonly IUrlEncoder _urlEncoder = urlEncoder;

    public async Task<Result> Handle(ResetPasswordCommand command, CancellationToken cancellationToken)
    {
        var user = await _unitOfWork.Users.FindByEmailAsync(command.Request.Email, cancellationToken);

        if (user is null || !await _unitOfWork.Users.IsEmailConfirmedAsync(user))
            return Result.Failure(UserErrors.InvalidCode);

        if (_urlEncoder.Decode(command.Request.Token) is not { } token)
            return Result.Failure(UserErrors.InvalidToken);

        var result = await _unitOfWork.Users.ResetPasswordAsync(user, token, command.Request.NewPassword);

        if (result.IsFailure)
            return Result.Failure(result.Error);

        return Result.Success();
    }
}
