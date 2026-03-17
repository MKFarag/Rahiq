namespace Application.Feathers.Payments.AddOrderPayment;

public record AddOrderPaymentCommand(int OrderId, FileData Image) : IRequest<Result>;
