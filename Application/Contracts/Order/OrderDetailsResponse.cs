namespace Application.Contracts.Order;

public record OrderDetailsResponse(
    int Id,
    string CustomerId,
    DateTime OrderDate,
    IEnumerable<OrderItemResponse> OrderItems,
    decimal Total,
    decimal GrandTotal,
    decimal Remaining,
    string Status,
    OrderShippingResponse? Shipping,
    OrderPaymentResponse? Payment
);
