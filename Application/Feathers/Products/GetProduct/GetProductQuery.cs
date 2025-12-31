namespace Application.Feathers.Products.GetProduct;

public record GetProductQuery(int Id) : IRequest<Result<ProductResponse>>;
