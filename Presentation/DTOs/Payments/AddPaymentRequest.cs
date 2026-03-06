namespace Presentation.DTOs.Payments;

public record AddPaymentRequest(
    decimal Amount,
    IFormFile Image
);

#region Validation

public class AddPaymentRequestValidator : AbstractValidator<AddPaymentRequest>
{
    public AddPaymentRequestValidator()
    {
        RuleFor(x => x.Amount)
            .GreaterThan(0)
            .PrecisionScale(10, 2, true);

        RuleFor(x => x.Image)
            .SetValidator(new FileSizeValidator())
            .SetValidator(new BlockedSignaturesValidator())
            .SetValidator(new ImageExtensionValidator());
    }
}

#endregion
