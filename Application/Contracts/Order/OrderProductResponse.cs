namespace Application.Contracts.Order;

public record OrderProductResponse(
    int Id,
    string Name,
    decimal UnitPrice,
    string? ImageUrl
);
