using Application.Contracts.Bundles;

namespace Presentation.DTOs.Bundles;

public record BundleWithImageRequest(
    BundleRequest Bundle,
    IFormFile? Image
);

#region Validation

public class BundleWithImageRequestValidator : AbstractValidator<BundleWithImageRequest>
{
    public BundleWithImageRequestValidator()
    {
        RuleFor(x => x.Bundle)
            .SetValidator(new BundleRequestValidator());

        When(x => x.Image is not null, () =>
            RuleFor(x => x.Image)
                .SetValidator(new FileSizeValidator()!)
                .SetValidator(new BlockedSignaturesValidator()!)
                .SetValidator(new ImageExtensionValidator()!));
    }
}

#endregion

