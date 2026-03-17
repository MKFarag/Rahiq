namespace Application.Feathers.Orders.CancelMyOrder;

public class CancelMyOrderCommandHandler(IUnitOfWork unitOfWork) : IRequestHandler<CancelMyOrderCommand, Result>
{
    private readonly IUnitOfWork _unitOfWork = unitOfWork;

    public async Task<Result> Handle(CancelMyOrderCommand request, CancellationToken cancellationToken = default)
    {
        if (await _unitOfWork.Orders.GetAsync([request.OrderId], cancellationToken) is not { } order)
            return Result.Failure(OrderErrors.NotFound);

        if (order.CustomerId != request.UserId)
            return Result.Failure(OrderErrors.InvalidPermission);

        if (!order.CanBeCancelled)
            return Result.Failure(OrderErrors.CannotBeCancelled);

        order.Cancel();

        await _unitOfWork.CompleteAsync(cancellationToken);

        return Result.Success();
    }
}
