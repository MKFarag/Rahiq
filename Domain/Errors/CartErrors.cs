namespace Domain.Errors;

public record CartErrors
{
    public static readonly Error InvalidQuantity =
        new("Cart.InvalidQuantity", "Invalid quantity", StatusCodes.BadRequest);

    public static readonly Error Empty =
        new("Cart.Empty", "The cart is empty", StatusCodes.BadRequest);
}
