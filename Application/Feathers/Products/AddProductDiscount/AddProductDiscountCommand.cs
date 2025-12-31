namespace Application.Feathers.Products.AddProductDiscount;

public record AddProductDiscountCommand(int Id, int Value) : IRequest<Result>;
