namespace Application.Contracts.Order;

public record OrderItemResponse(
    OrderProductResponse Product,
    int Quantity,
    decimal TotalPrice
);
