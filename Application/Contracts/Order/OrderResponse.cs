namespace Application.Contracts.Order;

public record OrderResponse(
    int Id,
    DateOnly OrderDate,
    int NumberOfItems,
    decimal Total
);
