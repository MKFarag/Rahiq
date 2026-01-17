namespace Application.Contracts.Shippings;

public record AdminShippingRequest(
    decimal Cost,
    string Code
);

#region Validation

public class AdminShippingRequestValidator : AbstractValidator<AdminShippingRequest>
{
    public AdminShippingRequestValidator()
    {
        RuleFor(x => x.Cost)
            .GreaterThanOrEqualTo(0)
            .PrecisionScale(4, 2, true);

        RuleFor(x => x.Code)
            .NotEmpty()
            .MaximumLength(50);
    }
}

#endregion

