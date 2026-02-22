namespace Application.Contracts.Carts;

public record UpdateCartRequest(
    int Quantity
);

#region Validation

public class UpdateCartProductRequestValidator : AbstractValidator<UpdateCartRequest>
{
    public UpdateCartProductRequestValidator()
    {
        RuleFor(x => x.Quantity)
            .GreaterThan(0);
    }
}

#endregion