namespace Presentation.DTOs.Payments;

public record AddPaymentRequest(
    IFormFile Image
);

#region Validation

public class AddPaymentRequestValidator : AbstractValidator<AddPaymentRequest>
{
    public AddPaymentRequestValidator()
    {
        RuleFor(x => x.Image)
            .SetValidator(new FileSizeValidator())
            .SetValidator(new BlockedSignaturesValidator())
            .SetValidator(new ImageExtensionValidator());
    }
}

#endregion
