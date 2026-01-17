namespace Application.Feathers.Shippings.AddCustomerShipping;

public record AddCustomerShippingCommand(CustomerShippingRequest Request) : IRequest<Result<ShippingResponse>>;

