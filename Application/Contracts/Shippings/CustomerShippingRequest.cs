namespace Application.Contracts.Shippings;

public record CustomerShippingRequest(
    string Address,
    string Phone
);

#region Validation

public class CustomerShippingRequestValidator : AbstractValidator<CustomerShippingRequest>
{
    public CustomerShippingRequestValidator()
    {
        RuleFor(x => x.Address)
            .NotEmpty()
            .MaximumLength(200);

        RuleFor(x => x.Phone)
            .NotEmpty()
            .MaximumLength(11)
            .Matches(RegexPatterns.OnlyNumbers)
            .WithMessage("Phone must contain only numbers.");
    }
}

#endregion
