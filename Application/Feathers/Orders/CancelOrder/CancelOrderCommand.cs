namespace Application.Feathers.Orders.CancelOrder;

public record CancelOrderCommand(int Id) : IRequest<Result>;
