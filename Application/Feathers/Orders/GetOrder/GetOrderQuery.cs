namespace Application.Feathers.Orders.GetOrder;

public record GetOrderQuery(int Id) : IRequest<Result<OrderResponse>>;

