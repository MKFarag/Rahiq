namespace Application.Contracts.Order;

public record OrderShippingResponse(
    string Id,
    string Name,
    string Address,
    string? Phone,
    decimal Cost,
    string Code
);
