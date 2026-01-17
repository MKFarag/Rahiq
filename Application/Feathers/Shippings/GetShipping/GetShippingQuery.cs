namespace Application.Feathers.Shippings.GetShipping;

public record GetShippingQuery(int Id) : IRequest<Result<ShippingResponse>>;

