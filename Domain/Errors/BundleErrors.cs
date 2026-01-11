namespace Domain.Errors;

public record BundleErrors
{
    public static readonly Error NotFound =
        new("Bundle.NotFound", "Nothing found by this Id", StatusCodes.NotFound);

    public static readonly Error DuplicatedName =
        new("Bundle.DuplicatedName", "This name is already exist", StatusCodes.Conflict);
}
