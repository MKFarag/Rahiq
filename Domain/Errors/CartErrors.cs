namespace Domain.Errors;

public record CartErrors
{
    public static readonly Error InvalidQuantity =
        new("Cart.InvalidQuantity", "Invalid quantity", StatusCodes.BadRequest);
}
