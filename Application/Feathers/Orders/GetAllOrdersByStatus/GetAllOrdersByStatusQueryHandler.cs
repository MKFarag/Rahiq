namespace Application.Feathers.Orders.GetAllOrdersByStatus;

public class GetAllOrdersByStatusQueryHandler(IUnitOfWork unitOfWork) : IRequestHandler<GetAllOrdersByStatusQuery, IPaginatedList<OrderResponse>>
{
    private readonly IUnitOfWork _unitOfWork = unitOfWork;

    public async Task<IPaginatedList<OrderResponse>> Handle(GetAllOrdersByStatusQuery request, CancellationToken cancellationToken = default)
    {
        if (!Enum.TryParse<OrderStatus>(request.Status, true, out var status))
            return EmptyPaginatedList.Create<OrderResponse>();

        var orders = await _unitOfWork.Orders
            .FindPaginatedListAsync<OrderResponse>
            (
                x => x.Status == status,
                request.Filters.PageNumber,
                request.Filters.PageSize,
                [nameof(Order.Shipping), nameof(Order.Payment), $"{nameof(Order.OrderItems)}.{nameof(OrderItem.Bundle)}", $"{nameof(Order.OrderItems)}.{nameof(OrderItem.Product)}"],
                cancellationToken
            );

        return orders;
    }
}
