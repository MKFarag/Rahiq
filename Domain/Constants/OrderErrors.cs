namespace Domain.Constants;

public record OrderErrors
{
    public static readonly Error NotFound =
        new("Order.NotFound", "No order found", StatusCodes.NotFound);

    public static readonly Error InvalidPermission =
        new("Order.InvalidPermission", "You cannot access this order", StatusCodes.Forbidden);
}
