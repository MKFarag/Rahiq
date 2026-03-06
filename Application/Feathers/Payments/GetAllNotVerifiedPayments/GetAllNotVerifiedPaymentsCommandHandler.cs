namespace Application.Feathers.Payments.GetAllNotVerifiedPayments;

public class GetAllNotVerifiedPaymentsCommandHandler(IUnitOfWork unitOfWork) : IRequestHandler<GetAllNotVerifiedPaymentsCommand, IEnumerable<PaymentResponse>>
{
    private readonly IUnitOfWork _unitOfWork = unitOfWork;

    public Task<IEnumerable<PaymentResponse>> Handle(GetAllNotVerifiedPaymentsCommand request, CancellationToken cancellationToken = default)
        => _unitOfWork.Orders
            .FindAllProjectionAsync<PaymentResponse>
            (
                x => x.Payment != null && !x.Payment.IsProofed && x.Status == OrderStatus.Paid,
                [nameof(Order.Payment)],
                cancellationToken
            );
}
