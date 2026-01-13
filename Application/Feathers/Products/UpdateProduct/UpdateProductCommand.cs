namespace Application.Feathers.Products.UpdateProduct;

public record UpdateProductCommand(int Id, ProductRequest Request) : IRequest<Result>;

