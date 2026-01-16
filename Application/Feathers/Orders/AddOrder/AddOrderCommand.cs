namespace Application.Feathers.Orders.AddOrder;

public record AddOrderCommand(string UserId) : IRequest<Result<OrderResponse>>;

