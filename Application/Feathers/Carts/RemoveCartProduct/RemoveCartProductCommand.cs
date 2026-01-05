namespace Application.Feathers.Carts.RemoveCartProduct;

public record RemoveCartProductCommand(string UserId, int ProductId) : IRequest<Result>;
