namespace Domain.Errors;

public record ProductErrors
{
    public static readonly Error NotFound =
        new("Product.NotFound", "Nothing found by this Id", StatusCodes.NotFound);

    public static readonly Error Percentage =
        new("Product.Percentage", "Invalid percentage value", StatusCodes.BadRequest);
}
