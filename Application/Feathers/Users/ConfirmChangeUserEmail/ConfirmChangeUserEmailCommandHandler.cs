namespace Application.Feathers.Users.ConfirmChangeUserEmail;

public class ConfirmChangeUserEmailCommandHandler(IUnitOfWork unitOfWork, IUrlEncoder urlEncoder, INotificationService notificationService) : IRequestHandler<ConfirmChangeUserEmailCommand, Result>
{
    private readonly IUnitOfWork _unitOfWork = unitOfWork;
    private readonly IUrlEncoder _urlEncoder = urlEncoder;
    private readonly INotificationService _notificationService = notificationService;

    public async Task<Result> Handle(ConfirmChangeUserEmailCommand command, CancellationToken cancellationToken = default)
    {
        if (await _unitOfWork.Users.FindByIdAsync(command.UserId, cancellationToken) is not { } user)
            return Result.Failure(UserErrors.NotFound);

        if (_urlEncoder.Decode(command.Request.Token) is not { } token)
            return Result.Failure(UserErrors.InvalidToken);

        var oldEmail = user.Email;

        var result = await _unitOfWork.Users.ChangeEmailAsync(user, command.Request.NewEmail, token);

        if (result.IsFailure)
            return result;

        await _notificationService.SendChangeEmailNotificationAsync(user, oldEmail, DateTime.UtcNow);

        return Result.Success();
    }
}
