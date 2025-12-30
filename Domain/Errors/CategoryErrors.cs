namespace Domain.Errors;

public record CategoryErrors
{
    public static readonly Error NotFound =
        new("Category.NotFound", "Nothing found by this Id", StatusCodes.NotFound);

    public static readonly Error DuplicatedName =
        new("Category.DuplicatedName", "This name is already exist", StatusCodes.Conflict);

    public static readonly Error InUse =
        new("Category.InUse", "Cannot delete this category because it has products assigned to it.", StatusCodes.Conflict);
}
