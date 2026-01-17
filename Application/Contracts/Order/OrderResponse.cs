namespace Application.Contracts.Order;

public record OrderResponse(
    int Id,
    DateOnly OrderDate,
    IEnumerable<OrderItemResponse> OrderItems,
    decimal Total,
    OrderStatus Status,
    OrderShippingResponse? Shipping,
    decimal? GrandTotal
);
