namespace Application.Contracts.Carts;

public record CartResponse(
    IEnumerable<CartProductResponse> CartProducts,
    IEnumerable<CartBundleResponse> CartBundle,
    decimal TotalPrice
);
