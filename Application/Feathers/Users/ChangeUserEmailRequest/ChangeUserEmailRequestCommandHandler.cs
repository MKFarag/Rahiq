namespace Application.Feathers.Users.ChangeUserEmailRequest;

public class ChangeUserEmailRequestCommandHandler(IUnitOfWork unitOfWork, IUrlEncoder urlEncoder, INotificationService notificationService) : IRequestHandler<ChangeUserEmailRequestCommand, Result>
{
    private readonly IUnitOfWork _unitOfWork = unitOfWork;
    private readonly IUrlEncoder _urlEncoder = urlEncoder;
    private readonly INotificationService _notificationService = notificationService;

    public async Task<Result> Handle(ChangeUserEmailRequestCommand request, CancellationToken cancellationToken = default)
    {
        if (await _unitOfWork.Users.FindByIdAsync(request.UserId, cancellationToken) is not { } user)
            return Result.Failure(UserErrors.NotFound);

        if (await _unitOfWork.Users.EmailExistsAsync(request.NewEmail, cancellationToken))
            return Result.Failure(UserErrors.DuplicatedEmail);

        if (user.Email.Equals(request.NewEmail, StringComparison.CurrentCultureIgnoreCase))
            return Result.Failure(UserErrors.SameEmail);

        var token = await _unitOfWork.Users.GenerateChangeEmailTokenAsync(user, request.NewEmail);
        token = _urlEncoder.Encode(token);

        await _notificationService.SendConfirmationLinkAsync(user, token);

        return Result.Success();
    }
}
