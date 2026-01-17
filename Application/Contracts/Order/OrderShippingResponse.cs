namespace Application.Contracts.Order;

public record OrderShippingResponse(
    string Id,
    string Address,
    string Phone,
    decimal? Cost,
    string? Code
);
