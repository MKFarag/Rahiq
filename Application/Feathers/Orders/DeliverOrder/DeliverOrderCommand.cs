namespace Application.Feathers.Orders.DeliverOrder;

public record DeliverOrderCommand(int OrderId) : IRequest<Result>;

