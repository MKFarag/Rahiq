namespace Application.Feathers.Products.AddProduct;

public record AddProductCommand(ProductRequest Request, FileData Image) : IRequest<Result<ProductResponse>>;

