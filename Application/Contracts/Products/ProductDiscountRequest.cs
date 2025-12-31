namespace Application.Contracts.Products;

public record ProductDiscountRequest(
    int DiscountPercentage
);

#region Validation

public class ProductDiscountRequestValidator : AbstractValidator<ProductDiscountRequest>
{
    public ProductDiscountRequestValidator()
    {
        RuleFor(x => x.DiscountPercentage)
            .NotEmpty()
            .InclusiveBetween(0, 100);
    }
}

#endregion