namespace Application.Feathers.Payments.VerifyPayment;

public record VerifyPaymentCommand(int PaymentId, decimal Amount) : IRequest<Result>;
