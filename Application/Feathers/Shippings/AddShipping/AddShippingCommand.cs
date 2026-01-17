namespace Application.Feathers.Shippings.AddShipping;

public record AddShippingCommand(CustomerShippingRequest Request) : IRequest<Result<ShippingResponse>>;

