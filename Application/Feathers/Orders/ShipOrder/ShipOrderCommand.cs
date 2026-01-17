namespace Application.Feathers.Orders.ShipOrder;

public record ShipOrderCommand(int OrderId, int ShippingId) : IRequest<Result>;

