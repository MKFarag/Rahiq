namespace Application.Feathers.Shippings.DeleteShipping;

public record DeleteShippingCommand(int Id) : IRequest<Result>;

