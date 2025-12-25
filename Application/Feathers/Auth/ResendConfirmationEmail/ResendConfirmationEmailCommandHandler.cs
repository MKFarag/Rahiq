namespace Application.Feathers.Auth.ResendConfirmationEmail;

public class ResendConfirmationEmailCommandHandler(INotificationService notificationService, IUnitOfWork unitOfWork, IUrlEncoder urlEncoder) : IRequestHandler<ResendConfirmationEmailCommand, Result>
{
    private readonly INotificationService _notificationService = notificationService;
    private readonly IUnitOfWork _unitOfWork = unitOfWork;
    private readonly IUrlEncoder _urlEncoder = urlEncoder;

    public async Task<Result> Handle(ResendConfirmationEmailCommand request, CancellationToken cancellationToken)
    {
        if (await _unitOfWork.Users.FindByEmailAsync(request.Email, cancellationToken) is not { } user)
            return Result.Success();

        if (await _unitOfWork.Users.IsEmailConfirmedAsync(user))
            return Result.Failure(UserErrors.DuplicatedConfirmation);

        var token = await _unitOfWork.Users.GenerateEmailConfirmationTokenAsync(user);
        token = _urlEncoder.Encode(token);
        await _notificationService.SendConfirmationLinkAsync(user, token);

        return Result.Success();
    }
}
