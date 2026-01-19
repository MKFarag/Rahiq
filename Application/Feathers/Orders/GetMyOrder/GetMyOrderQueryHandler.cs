namespace Application.Feathers.Orders.GetMyOrder;

public class GetMyOrderQueryHandler(IUnitOfWork unitOfWork) : IRequestHandler<GetMyOrderQuery, Result<OrderResponse>>
{
    private readonly IUnitOfWork _unitOfWork = unitOfWork;

    public async Task<Result<OrderResponse>> Handle(GetMyOrderQuery request, CancellationToken cancellationToken = default)
    {
        var order = await _unitOfWork.Orders
            .FindAsync
            (
                x => x.Id == request.OrderId,
                [nameof(Order.Shipping), nameof(Order.Payment), $"{nameof(Order.OrderItems)}.{nameof(OrderItem.Bundle)}", $"{nameof(Order.OrderItems)}.{nameof(OrderItem.Product)}"],
                cancellationToken
            );

        if (order is null)
            return Result.Failure<OrderResponse>(OrderErrors.NotFound);

        if (order.CustomerId != request.UserId)
            return Result.Failure<OrderResponse>(OrderErrors.InvalidPermission);

        return Result.Success(order.Adapt<OrderResponse>());
    }
}
