namespace Application.Feathers.Orders.CancelOrder;

public record CancelOrderCommand(int OrderId, string UserId) : IRequest<Result>;

