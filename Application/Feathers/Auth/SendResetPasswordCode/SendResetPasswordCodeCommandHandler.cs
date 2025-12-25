namespace Application.Feathers.Auth.SendResetPasswordCode;

public class SendResetPasswordCodeCommandHandler(INotificationService notificationService, IUnitOfWork unitOfWork, IUrlEncoder urlEncoder) : IRequestHandler<SendResetPasswordCodeCommand, Result>
{
    private readonly INotificationService _notificationService = notificationService;
    private readonly IUnitOfWork _unitOfWork = unitOfWork;
    private readonly IUrlEncoder _urlEncoder = urlEncoder;

    public async Task<Result> Handle(SendResetPasswordCodeCommand request, CancellationToken cancellationToken)
    {
        if (await _unitOfWork.Users.FindByEmailAsync(request.Email, cancellationToken) is not { } user)
            return Result.Success();

        if (!await _unitOfWork.Users.IsEmailConfirmedAsync(user))
            return Result.Failure(UserErrors.EmailNotConfirmed with { StatusCode = StatusCodes.BadRequest });

        var token = await _unitOfWork.Users.GeneratePasswordResetTokenAsync(user);
        token = _urlEncoder.Encode(token);

        await _notificationService.SendResetPasswordAsync(user, token);

        return Result.Success();
    }
}
