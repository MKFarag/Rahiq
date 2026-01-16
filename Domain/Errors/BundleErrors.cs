namespace Domain.Errors;

public record BundleErrors
{
    public static readonly Error NotFound =
        new("Bundle.NotFound", "Nothing found by this Id", StatusCodes.NotFound);

    public static readonly Error DuplicatedName =
        new("Bundle.DuplicatedName", "This name is already exist", StatusCodes.Conflict);

    public static readonly Error NoImage =
        new("Bundle.NoImage", "No image is exist", StatusCodes.NotFound);

    public static readonly Error ImageExist =
        new("Bundle.ImageExist", "This bundle already has an image", StatusCodes.BadRequest);

    public static readonly Error IsActive =
        new("Bundle.IsActive", "This bundle is already active", StatusCodes.BadRequest);

    public static readonly Error NotActive =
        new("Bundle.NotActive", "This bundle is currently unavailable", StatusCodes.BadRequest);

    public static readonly Error DuplicatedProducts =
        new("Bundle.DuplicatedProducts", "Another bundle already contains the same products", StatusCodes.Conflict);

    public static readonly Error InvalidQuantity =
        new("Bundle.InvalidQuantity", "Invalid quantity", StatusCodes.Conflict);
}
