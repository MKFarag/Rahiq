namespace Application.Feathers.Carts.UpdateCart;

public record UpdateCartCommand(string UserId, int CartId, int Quantity) : IRequest<Result>;
