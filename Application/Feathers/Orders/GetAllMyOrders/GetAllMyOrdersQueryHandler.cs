namespace Application.Feathers.Orders.GetAllMyOrders;

public class GetAllMyOrdersQueryHandler(IUnitOfWork unitOfWork) : IRequestHandler<GetAllMyOrdersQuery, IEnumerable<OrderResponse>>
{
    private readonly IUnitOfWork _unitOfWork = unitOfWork;

    public async Task<IEnumerable<OrderResponse>> Handle(GetAllMyOrdersQuery request, CancellationToken cancellationToken = default)
    {
        if (request.Year < 2026)
            return [];

        var orders = await _unitOfWork.Orders
            .FindAllProjectionAsync<OrderResponse>
            (
                x => x.CustomerId == request.UserId && x.Date.Year == request.Year,
                [nameof(Order.Shipping), $"{nameof(Order.OrderItems)}.{nameof(OrderItem.Bundle)}", $"{nameof(Order.OrderItems)}.{nameof(OrderItem.Product)}"],
                cancellationToken
            );

        return orders;
    }
}
