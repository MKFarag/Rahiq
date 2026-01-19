namespace Application.Feathers.Payments.VerifyPayment;

public class VerifyPaymentCommandHandler(IUnitOfWork unitOfWork) : IRequestHandler<VerifyPaymentCommand, Result>
{
    private readonly IUnitOfWork _unitOfWork = unitOfWork;

    public async Task<Result> Handle(VerifyPaymentCommand request, CancellationToken cancellationToken = default)
    {
        if (await _unitOfWork.Payments.GetAsync([request.PaymentId], cancellationToken) is not { } payment)
            return Result.Failure(PaymentErrors.NotFound);

        if (payment.IsProofed)
            return Result.Failure(PaymentErrors.AlreadyVerified);

        payment.IsProofed = true;

        await _unitOfWork.CompleteAsync(cancellationToken);

        return Result.Success();
    }
}
