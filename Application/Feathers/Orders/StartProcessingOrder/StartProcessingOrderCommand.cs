namespace Application.Feathers.Orders.StartProcessingOrder;

public record StartProcessingOrderCommand(int OrderId) : IRequest<Result>;

