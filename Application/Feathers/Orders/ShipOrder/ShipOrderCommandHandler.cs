namespace Application.Feathers.Orders.ShipOrder;

public class ShipOrderCommandHandler(IUnitOfWork unitOfWork) : IRequestHandler<ShipOrderCommand, Result>
{
    private readonly IUnitOfWork _unitOfWork = unitOfWork;

    public async Task<Result> Handle(ShipOrderCommand request, CancellationToken cancellationToken = default)
    {
        if (await _unitOfWork.Orders.GetAsync([request.OrderId], cancellationToken) is not { } order)
            return Result.Failure(OrderErrors.NotFound);

        if (order.Status != OrderStatus.Processing)
            return Result.Failure(OrderErrors.InvalidStatusTransition);

        if (!await _unitOfWork.Shipping.AnyAsync(x => x.Id == request.ShippingId, cancellationToken))
            return Result.Failure(ShippingErrors.NotFound);

        order.Ship(request.ShippingId);

        await _unitOfWork.CompleteAsync(cancellationToken);

        return Result.Success();
    }
}

