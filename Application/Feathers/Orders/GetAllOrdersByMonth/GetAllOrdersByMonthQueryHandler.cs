namespace Application.Feathers.Orders.GetAllOrdersByMonth;

public class GetAllOrdersByMonthQueryHandler(IUnitOfWork unitOfWork) : IRequestHandler<GetAllOrdersByMonthQuery, IPaginatedList<OrderResponse>>
{
    private readonly IUnitOfWork _unitOfWork = unitOfWork;

    public async Task<IPaginatedList<OrderResponse>> Handle(GetAllOrdersByMonthQuery request, CancellationToken cancellationToken = default)
    {
        if (request.Month is > 12 or < 1)
            return EmptyPaginatedList.Create<OrderResponse>();

        var orders = await _unitOfWork.Orders
            .FindPaginatedListAsync<OrderResponse>
            (
                x => x.Date.Month == request.Month,
                request.Filters.PageNumber,
                request.Filters.PageSize,
                [nameof(Order.Shipping), $"{nameof(Order.OrderItems)}.{nameof(OrderItem.Bundle)}", $"{nameof(Order.OrderItems)}.{nameof(OrderItem.Product)}"],
                cancellationToken
            );

        return orders;
    }
}
