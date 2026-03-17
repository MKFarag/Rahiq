namespace Application.Feathers.Orders.CancelMyOrder;

public record CancelMyOrderCommand(int OrderId, string UserId) : IRequest<Result>;

