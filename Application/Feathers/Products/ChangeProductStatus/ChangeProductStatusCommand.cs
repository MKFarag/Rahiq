namespace Application.Feathers.Products.ChangeProductStatus;

public record ChangeProductStatusCommand(int Id) : IRequest<Result>;
