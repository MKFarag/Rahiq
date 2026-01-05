namespace Application.Contracts.Carts;

public record UpdateCartProductRequest(
    int ProductId,
    int Quantity
);

#region Validation

public class UpdateCartProductRequestValidator : AbstractValidator<UpdateCartProductRequest>
{
    public UpdateCartProductRequestValidator()
    {
        RuleFor(x => x.ProductId)
            .NotEmpty()
            .GreaterThan(0);

        RuleFor(x => x.Quantity)
            .NotEmpty()
            .GreaterThan(0);
    }
}

#endregion