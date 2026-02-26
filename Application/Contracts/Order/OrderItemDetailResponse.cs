namespace Application.Contracts.Order;

public record OrderItemDetailResponse(
    int Id,
    string Name,
    decimal UnitPrice,
    string? ImageUrl
);
