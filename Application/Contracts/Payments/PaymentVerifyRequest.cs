namespace Application.Contracts.Payments;

public record PaymentVerifyRequest(
    decimal Amount
);

#region Validation

public class PaymentVerifyRequestValidator : AbstractValidator<PaymentVerifyRequest>
{
    public PaymentVerifyRequestValidator()
    {
        RuleFor(x => x.Amount)
            .GreaterThan(0)
            .PrecisionScale(10, 2, true);
    }
}

#endregion
