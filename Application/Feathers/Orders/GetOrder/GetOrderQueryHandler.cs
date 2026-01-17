namespace Application.Feathers.Orders.GetOrder;

public class GetOrderQueryHandler(IUnitOfWork unitOfWork) : IRequestHandler<GetOrderQuery, Result<OrderResponse>>
{
    private readonly IUnitOfWork _unitOfWork = unitOfWork;

    public async Task<Result<OrderResponse>> Handle(GetOrderQuery request, CancellationToken cancellationToken = default)
    {
        var order = await _unitOfWork.Orders
            .FindAsync
            (
                o => o.Id == request.Id,
                [nameof(Order.Shipping), $"{nameof(Order.OrderItems)}.{nameof(OrderItem.Bundle)}", $"{nameof(Order.OrderItems)}.{nameof(OrderItem.Product)}"],
                cancellationToken
            );

        return order is null
            ? Result.Failure<OrderResponse>(OrderErrors.NotFound)
            : Result.Success(order.Adapt<OrderResponse>());
    }
}
