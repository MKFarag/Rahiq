namespace Domain.Constants;

public record OrderErrors
{
    public static readonly Error NotFound =
        new("Order.NotFound", "No order found", StatusCodes.NotFound);

    public static readonly Error InvalidPermission =
        new("Order.InvalidPermission", "You cannot access this order", StatusCodes.Forbidden);

    public static readonly Error InvalidStatusTransition =
        new("Order.InvalidStatusTransition", "Cannot perform this action on the order in its current status", StatusCodes.Conflict);

    public static readonly Error CannotBeCancelled =
        new("Order.CannotBeCancelled", "This order can no longer be cancelled", StatusCodes.Conflict);
}
