namespace Application.Feathers.Carts.ClearMyCart;

public record ClearMyCartCommand(string UserId) : IRequest<Result>;
