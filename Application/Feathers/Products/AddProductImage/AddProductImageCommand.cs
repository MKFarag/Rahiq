namespace Application.Feathers.Products.AddProductImage;

public record AddProductImageCommand(int Id, FileData Image) : IRequest<Result>;

