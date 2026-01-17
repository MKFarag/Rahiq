namespace Application.Contracts.Shippings;

public record ShippingRequest(
    string Address,
    string Phone,
    decimal? Cost,
    string? Code
);

#region Validation

public class ShippingRequestValidator : AbstractValidator<ShippingRequest>
{
    public ShippingRequestValidator()
    {
        RuleFor(x => x.Address)
            .NotEmpty()
            .MaximumLength(200);

        RuleFor(x => x.Phone)
            .NotEmpty()
            .MaximumLength(15)
            .Matches(RegexPatterns.OnlyNumbers)
            .WithMessage("Phone must contain only numbers.");

        When(x => x.Cost is not null, () => 
            RuleFor(x => x.Cost)
                .GreaterThanOrEqualTo(0)
                .PrecisionScale(4, 2, true)
        );

        RuleFor(x => x.Code)
            .MaximumLength(50)
            .When(x => !string.IsNullOrEmpty(x.Code));
    }
}

#endregion