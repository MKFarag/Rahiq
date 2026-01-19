namespace Application.Feathers.Payments.AddOrderPayment;

public record AddOrderPaymentCommand(int OrderId, decimal Amount, FileData Image) : IRequest<Result>;
