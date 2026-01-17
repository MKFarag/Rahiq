namespace Application.Feathers.Shippings.AssignShippingDetails;

public record AssignShippingDetailsCommand(int Id, AdminShippingRequest Request) : IRequest<Result>;

