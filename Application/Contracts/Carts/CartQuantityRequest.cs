namespace Application.Contracts.Carts;

public record CartQuantityRequest(
    int ItemId,
    int Quantity,
    bool IsBundle
);

#region Validation

public class CartQuantityRequestValidator : AbstractValidator<CartQuantityRequest>
{
    public CartQuantityRequestValidator()
    {
        RuleFor(x => x.Quantity)
            .GreaterThan(0);

        RuleFor(x => x.ItemId)
            .NotEmpty();
    }
}

#endregion