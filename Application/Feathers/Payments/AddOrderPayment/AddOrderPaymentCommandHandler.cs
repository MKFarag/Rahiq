namespace Application.Feathers.Payments.AddOrderPayment;

public class AddOrderPaymentCommandHandler(IUnitOfWork unitOfWork, IFileStorageService fileStorageService, INotificationService notificationService) : IRequestHandler<AddOrderPaymentCommand, Result>
{
    private readonly IUnitOfWork _unitOfWork = unitOfWork;
    private readonly IFileStorageService _fileStorageService = fileStorageService;
    private readonly INotificationService _notificationService = notificationService;

    public async Task<Result> Handle(AddOrderPaymentCommand request, CancellationToken cancellationToken = default)
    {
        if (await _unitOfWork.Orders.GetAsync([request.OrderId], cancellationToken) is not { } order)
            return Result.Failure(OrderErrors.NotFound);

        if (order.Status != OrderStatus.Pending)
            return Result.Failure(OrderErrors.InvalidStatusTransition);

        if (order.PaymentId is not null)
            return Result.Failure(PaymentErrors.AlreadyExists);

        var fileName = $"payment-{order.Id}-{Path.GetFileName(request.Image.FileName)}{Path.GetExtension(request.Image.FileName)}";

        await _fileStorageService.SaveAsync(request.Image.Stream, fileName, cancellationToken);

        var relativePath = _fileStorageService.GetRelativePath(fileName);

        var payment = new Payment
        {
            ImageUrl = relativePath,
            Amount = request.Amount,
            IsProofed = false
        };

        await _unitOfWork.Payments.AddAsync(payment, cancellationToken);
        await _unitOfWork.CompleteAsync(cancellationToken);

        order.Pay(payment.Id);
        await _unitOfWork.CompleteAsync(cancellationToken);

        return Result.Success();
    }
}
