namespace Domain.Errors;

public record TypeErrors
{
    public static readonly Error NotFound =
        new("Type.NotFound", "Nothing found by this Id", StatusCodes.BadRequest);

    public static readonly Error DuplicatedName =
        new("Type.DuplicatedName", "This name is already exist", StatusCodes.Conflict);

    public static readonly Error InUse =
        new("Type.InUse", "Cannot delete this type because it has products assigned to it.", StatusCodes.Conflict);
}
