namespace Application.Contracts.Carts;

public record CartBundleResponse(
    int Id,
    string Name,
    decimal UnitPrice,
    int Quantity,
    string? ImageUrl
);
