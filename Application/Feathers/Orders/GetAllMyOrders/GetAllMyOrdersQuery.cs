namespace Application.Feathers.Orders.GetAllMyOrders;

public record GetAllMyOrdersQuery(string UserId, int Year) : IRequest<IEnumerable<OrderResponse>>;

