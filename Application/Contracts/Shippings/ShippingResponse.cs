namespace Application.Contracts.Shippings;

public record ShippingResponse(
    int Id,
    string Address,
    string Phone,
    decimal? Cost,
    string? Code
);
