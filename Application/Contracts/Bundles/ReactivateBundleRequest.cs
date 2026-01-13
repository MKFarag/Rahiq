namespace Application.Contracts.Bundles;

public record ReactivateBundleRequest(
    int QuantityAvailable,
    DateOnly EndAt
);

#region Validation

public class ReactivateBundleRequestValidator : AbstractValidator<ReactivateBundleRequest>
{
    public ReactivateBundleRequestValidator()
    {
        RuleFor(x => x.QuantityAvailable)
            .NotEmpty()
            .ExclusiveBetween(0, 1000);

        RuleFor(x => x.EndAt)
            .NotEmpty()
            .GreaterThanOrEqualTo(DateOnly.FromDateTime(DateTime.Today))
            .WithMessage("You cannot enter a date before today.");
    }
}

#endregion