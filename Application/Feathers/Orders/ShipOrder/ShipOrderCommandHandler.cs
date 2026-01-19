namespace Application.Feathers.Orders.ShipOrder;

public class ShipOrderCommandHandler(IUnitOfWork unitOfWork) : IRequestHandler<ShipOrderCommand, Result>
{
    private readonly IUnitOfWork _unitOfWork = unitOfWork;

    public async Task<Result> Handle(ShipOrderCommand request, CancellationToken cancellationToken = default)
    {
        var order = await _unitOfWork.Orders
            .FindAsync(x => x.Id == request.OrderId, [nameof(Order.Payment)], cancellationToken);

        if (order is null)
            return Result.Failure(OrderErrors.NotFound);

        if (order.Status != OrderStatus.Processing)
            return Result.Failure(OrderErrors.InvalidStatusTransition);

        // لو فيه Payment لازم يكون متحقق منه قبل الشحن
        if (order.Payment is not null && !order.Payment.IsProofed)
            return Result.Failure(PaymentErrors.NotVerified);

        if (!await _unitOfWork.Shipping.AnyAsync(x => x.Id == request.ShippingId, cancellationToken))
            return Result.Failure(ShippingErrors.NotFound);

        order.Ship(request.ShippingId);

        await _unitOfWork.CompleteAsync(cancellationToken);

        return Result.Success();
    }
}

