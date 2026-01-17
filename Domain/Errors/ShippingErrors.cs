namespace Domain.Errors;

public record ShippingErrors
{
    public static readonly Error NotFound =
        new("Shipping.NotFound", "No shipping found", StatusCodes.NotFound);
}
