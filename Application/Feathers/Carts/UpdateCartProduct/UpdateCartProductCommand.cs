namespace Application.Feathers.Carts.UpdateCartProduct;

public record UpdateCartProductCommand(string UserId, int ProductId, int Quantity) : IRequest<Result>;
