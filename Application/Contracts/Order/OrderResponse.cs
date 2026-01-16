namespace Application.Contracts.Order;

public record OrderResponse(
    int Id,
    DateOnly OrderDate,
    IEnumerable<OrderItemResponse> OrderItems,
    decimal TotalAmount,
    OrderStatus Status,
    OrderShippingResponse? Shipping,
    decimal? GrandTotal
);
