namespace Application.Feathers.Carts.AddToCart;

public record AddToCartCommand(string UserId, int ItemId, int Quantity, bool IsBundle) : IRequest<Result>;

