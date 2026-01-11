namespace Application.Feathers.Products.DeleteProductImage;

public record DeleteProductImageCommand(int Id) : IRequest<Result>;

