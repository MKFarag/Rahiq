namespace Application.Feathers.Carts.RemoveCartItem;

public record RemoveCartItemCommand(int CartId, string UserId) : IRequest<Result>;

