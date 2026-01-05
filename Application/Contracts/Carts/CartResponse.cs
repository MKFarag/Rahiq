namespace Application.Contracts.Carts;

public record CartResponse(
    IEnumerable<CartProductResponse> CartProducts,
    decimal TotalPrice
);
