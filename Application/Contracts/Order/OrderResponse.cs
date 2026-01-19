namespace Application.Contracts.Order;

public record OrderResponse(
    int Id,
    DateOnly OrderDate,
    IEnumerable<OrderItemResponse> OrderItems,
    decimal Total,
    decimal GrandTotal,
    decimal Remaining,
    OrderStatus Status,
    OrderShippingResponse? Shipping,
    OrderPaymentResponse? Payment
);
