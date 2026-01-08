namespace Domain.Errors;

public record ProductErrors
{
    public static readonly Error NotFound =
        new("Product.NotFound", "Nothing found by this Id", StatusCodes.NotFound);

    public static readonly Error Percentage =
        new("Product.Percentage", "Invalid percentage value", StatusCodes.BadRequest);

    public static readonly Error NotAvailable =
        new("Product.NotAvailable", "This product is not available right now", StatusCodes.BadRequest);

    public static readonly Error DuplicatedName =
        new("Product.DuplicatedName", "This name is already exist", StatusCodes.Conflict);
}
