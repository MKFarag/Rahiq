using Application.Contracts.Products;

namespace Presentation.DTOs.Products;

public record ProductWithImageRequest(
    ProductRequest Product,
    IFormFile? Image
);

#region Validation

public class ProductWithImageRequestValidator : AbstractValidator<ProductWithImageRequest>
{
    public ProductWithImageRequestValidator()
    {
        RuleFor(x => x.Product)
            .SetValidator(new ProductRequestValidator());

        When(x => x.Image is not null, () =>
            RuleFor(x => x.Image)
                .SetValidator(new FileSizeValidator()!)
                .SetValidator(new BlockedSignaturesValidator()!)
                .SetValidator(new ImageExtensionValidator()!));
    }
}

#endregion