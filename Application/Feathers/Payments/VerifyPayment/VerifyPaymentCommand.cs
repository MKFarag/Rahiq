namespace Application.Feathers.Payments.VerifyPayment;

public record VerifyPaymentCommand(int PaymentId) : IRequest<Result>;
