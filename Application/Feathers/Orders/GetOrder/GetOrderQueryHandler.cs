namespace Application.Feathers.Orders.GetOrder;

public class GetOrderQueryHandler(IUnitOfWork unitOfWork) : IRequestHandler<GetOrderQuery, Result<OrderDetailsResponse>>
{
    private readonly IUnitOfWork _unitOfWork = unitOfWork;

    public async Task<Result<OrderDetailsResponse>> Handle(GetOrderQuery request, CancellationToken cancellationToken = default)
    {
        var order = await _unitOfWork.Orders
            .FindAsync
            (
                o => o.Id == request.Id,
                [nameof(Order.Shipping), nameof(Order.Payment), $"{nameof(Order.OrderItems)}.{nameof(OrderItem.Bundle)}", $"{nameof(Order.OrderItems)}.{nameof(OrderItem.Product)}"],
                cancellationToken
            );

        return order is null
            ? Result.Failure<OrderDetailsResponse>(OrderErrors.NotFound)
            : Result.Success(order.Adapt<OrderDetailsResponse>());
    }
}
