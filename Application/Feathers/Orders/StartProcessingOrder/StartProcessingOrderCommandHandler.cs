namespace Application.Feathers.Orders.StartProcessingOrder;

public class StartProcessingOrderCommandHandler(IUnitOfWork unitOfWork) : IRequestHandler<StartProcessingOrderCommand, Result>
{
    private readonly IUnitOfWork _unitOfWork = unitOfWork;

    public async Task<Result> Handle(StartProcessingOrderCommand request, CancellationToken cancellationToken = default)
    {
        if (await _unitOfWork.Orders.GetAsync([request.OrderId], cancellationToken) is not { } order)
            return Result.Failure(OrderErrors.NotFound);

        if (order.Status != OrderStatus.Pending)
            return Result.Failure(OrderErrors.InvalidStatusTransition);

        order.StartProcessing();

        await _unitOfWork.CompleteAsync(cancellationToken);

        return Result.Success();
    }
}

