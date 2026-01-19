namespace Domain.Errors;

public record PaymentErrors
{
    public static readonly Error NotFound =
        new("Payment.NotFound", "No payment found", StatusCodes.NotFound);

    public static readonly Error AlreadyExists =
        new("Payment.AlreadyExists", "This order already has a payment", StatusCodes.Conflict);

    public static readonly Error AlreadyVerified =
        new("Payment.AlreadyVerified", "This payment is already verified", StatusCodes.Conflict);

    public static readonly Error NotVerified =
        new("Payment.NotVerified", "Payment must be verified before shipping", StatusCodes.BadRequest);
}
