namespace Application.Contracts.Order;

public record OrderStatusRequest(
    string Status
);

#region Validation

public class OrderStatusRequestValidator : AbstractValidator<OrderStatusRequest>
{
    public OrderStatusRequestValidator()
    {
        RuleFor(x => x.Status)
            .NotEmpty()
            .Must(status =>
                Enum.TryParse<OrderStatus>(status, true, out _))
            .WithMessage("Invalid order status value");
    }
}

#endregion
