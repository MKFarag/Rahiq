namespace Application.Feathers.Auth.ConfirmEmail;

public class ConfirmEmailCommandHandler(IUnitOfWork unitOfWork, IUrlEncoder urlEncoder) : IRequestHandler<ConfirmEmailCommand, Result>
{
    private readonly IUnitOfWork _unitOfWork = unitOfWork;
    private readonly IUrlEncoder _urlEncoder = urlEncoder;

    public async Task<Result> Handle(ConfirmEmailCommand request, CancellationToken cancellationToken)
    {
        if (await _unitOfWork.Users.FindByIdAsync(request.UserId, cancellationToken) is not { } user)
            return Result.Failure(UserErrors.InvalidCode);

        if (await _unitOfWork.Users.IsEmailConfirmedAsync(user))
            return Result.Failure(UserErrors.DuplicatedConfirmation);

        if (_urlEncoder.Decode(request.Token) is not { } token)
            return Result.Failure(UserErrors.InvalidToken);

        var result = await _unitOfWork.Users.ConfirmEmailWithTokenAsync(user, token);

        if (result.IsFailure)
            return Result.Failure(result.Error);

        await _unitOfWork.Users.AddToRoleAsync(user, DefaultRoles.Customer.Name);

        return Result.Success();
    }
}
