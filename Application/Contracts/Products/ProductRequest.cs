namespace Application.Contracts.Products;

public record ProductRequest(
    string Name,
    string Brand,
    string Description,
    int CategoryId,
    string TypeId,
    decimal Price,
    int DiscountPercentage,
    bool IsAvailable
);

#region Validation

public class ProductRequestValidator : AbstractValidator<ProductRequest>
{
    public ProductRequestValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty()
            .MaximumLength(150);

        RuleFor(x => x.Brand)
            .NotEmpty()
            .MaximumLength(50);

        RuleFor(x => x.Description)
            .NotEmpty();

        RuleFor(x => x.Price)
            .NotEmpty()
            .PrecisionScale(5, 2, true);

        RuleFor(x => x.DiscountPercentage)
            .NotEmpty()
            .InclusiveBetween(0, 100);
    }
}

#endregion