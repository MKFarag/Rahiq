namespace Application.Contracts.Order;

public record OrderItemResponse(
    int Id,
    string Name,
    decimal UnitPrice,
    string? ImageUrl,
    int Quantity,
    decimal Total
);
