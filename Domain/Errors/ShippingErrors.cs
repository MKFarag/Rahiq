namespace Domain.Errors;

public record ShippingErrors
{
    public static readonly Error NotFound =
        new("Shipping.NotFound", "No shipping found", StatusCodes.NotFound);

    public static readonly Error DuplicatedCode =
        new("Shipping.DuplicatedCode", "A shipping with this code already exists", StatusCodes.Conflict);

    public static readonly Error InUse =
        new("Shipping.InUse", "Cannot delete shipping that is used in orders", StatusCodes.Conflict);
}
