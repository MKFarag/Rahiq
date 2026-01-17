namespace Application.Feathers.Orders.DeliverOrder;

public class DeliverOrderCommandHandler(IUnitOfWork unitOfWork) : IRequestHandler<DeliverOrderCommand, Result>
{
    private readonly IUnitOfWork _unitOfWork = unitOfWork;

    public async Task<Result> Handle(DeliverOrderCommand request, CancellationToken cancellationToken = default)
    {
        if (await _unitOfWork.Orders.GetAsync([request.OrderId], cancellationToken) is not { } order)
            return Result.Failure(OrderErrors.NotFound);

        if (order.Status != OrderStatus.Shipped)
            return Result.Failure(OrderErrors.InvalidStatusTransition);

        order.Deliver();

        await _unitOfWork.CompleteAsync(cancellationToken);

        return Result.Success();
    }
}

