namespace Application.Feathers.Shippings.AddCustomerShipping;

public record AddCustomerShippingCommand(CustomerShippingRequest Request, string UserId) : IRequest<Result<ShippingResponse>>;

