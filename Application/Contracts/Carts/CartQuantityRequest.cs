namespace Application.Contracts.Carts;

public record CartQuantityRequest(
    int Quantity,
    bool IsBundle
);

#region Validation

public class CartQuantityRequestValidator : AbstractValidator<CartQuantityRequest>
{
    public CartQuantityRequestValidator()
    {
        RuleFor(x => x.Quantity)
            .NotEmpty()
            .GreaterThan(0);

        RuleFor(x => x.IsBundle)
            .NotEmpty();
    }
}

#endregion