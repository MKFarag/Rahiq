namespace Application.Contracts.Carts;

public record CartProductResponse(
    int Id,
    string Name,
    decimal CurrentPrice,
    int Quantity,
    string MainImageUrl
);
