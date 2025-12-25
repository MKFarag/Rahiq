namespace Application.Feathers.Auth.Register;

public class RegisterCommandHandler(INotificationService notificationService, IUnitOfWork unitOfWork, IUrlEncoder urlEncoder) : IRequestHandler<RegisterCommand, Result>
{
    private readonly INotificationService _notificationService = notificationService;
    private readonly IUnitOfWork _unitOfWork = unitOfWork;
    private readonly IUrlEncoder _urlEncoder = urlEncoder;

    public async Task<Result> Handle(RegisterCommand command, CancellationToken cancellationToken)
    {
        if (await _unitOfWork.Users.EmailExistsAsync(command.Request.Email, cancellationToken))
            return Result.Failure(UserErrors.DuplicatedEmail);

        var user = command.Request.Adapt<User>();

        var result = await _unitOfWork.Users.CreateAsync(user, command.Request.Password);

        if (result.IsFailure)
            return Result.Failure(result.Error);

        var token = await _unitOfWork.Users.GenerateEmailConfirmationTokenAsync(user);
        token = _urlEncoder.Encode(token);
        await _notificationService.SendConfirmationLinkAsync(user, token);

        return Result.Success();
    }
}
