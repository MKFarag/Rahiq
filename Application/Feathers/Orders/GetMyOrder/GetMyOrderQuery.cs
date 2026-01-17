namespace Application.Feathers.Orders.GetMyOrder;

public record GetMyOrderQuery(int OrderId, string UserId) : IRequest<Result<OrderResponse>>;

