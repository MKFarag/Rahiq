namespace Application.Feathers.Orders.GetAllMyOrders;

public class GetAllMyOrdersQueryHandler(IUnitOfWork unitOfWork) : IRequestHandler<GetAllMyOrdersQuery, IPaginatedList<OrderResponse>>
{
    private readonly IUnitOfWork _unitOfWork = unitOfWork;

    public async Task<IPaginatedList<OrderResponse>> Handle(GetAllMyOrdersQuery request, CancellationToken cancellationToken = default)
    {
        if (request.Year < 2026)
            return EmptyPaginatedList.Create<OrderResponse>();

        var orders = await _unitOfWork.Orders
            .FindPaginatedListAsync<OrderResponse>
            (
                x => x.CustomerId == request.UserId && x.Date.Year == request.Year,
                request.Filters.PageNumber,
                request.Filters.PageSize,
                [nameof(Order.Shipping), nameof(Order.Payment), $"{nameof(Order.OrderItems)}.{nameof(OrderItem.Bundle)}", $"{nameof(Order.OrderItems)}.{nameof(OrderItem.Product)}"],
                cancellationToken
            );

        return orders;
    }
}
