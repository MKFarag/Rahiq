namespace Application.Feathers.Carts.GetMyCart;

public record GetMyCartQuery(string UserId) : IRequest<CartResponse>;
