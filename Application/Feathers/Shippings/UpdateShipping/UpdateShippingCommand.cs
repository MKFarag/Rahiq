namespace Application.Feathers.Shippings.UpdateShipping;

public record UpdateShippingCommand(int Id, ShippingRequest Request) : IRequest<Result>;

