namespace Application.Contracts.Bundles;

public record BundleRequest(
    string Name,
    int QuantityAvailable,
    int DiscountPercentage,
    DateOnly EndAt,
    IEnumerable<int> ProductsId
);

#region Validation

public class BundleRequestValidator : AbstractValidator<BundleRequest>
{
    public BundleRequestValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty()
            .MaximumLength(200);

        RuleFor(x => x.QuantityAvailable)
            .NotEmpty()
            .ExclusiveBetween(0, 1000);

        RuleFor(x => x.EndAt)
            .NotEmpty()
            .GreaterThanOrEqualTo(DateOnly.FromDateTime(DateTime.Today))
            .WithMessage("You cannot enter a date before today.");

        RuleFor(x => x.ProductsId)
            .NotEmpty()
            .Must(x => x.Count() > 1)
            .WithMessage("You must enter more than one product.");
    }
}

#endregion