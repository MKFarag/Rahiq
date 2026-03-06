namespace Application.Feathers.Payments.GetAllNotVerifiedPayments;

public record GetAllNotVerifiedPaymentsCommand() : IRequest<IEnumerable<PaymentResponse>>;

