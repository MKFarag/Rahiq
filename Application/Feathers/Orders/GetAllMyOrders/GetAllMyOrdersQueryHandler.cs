namespace Application.Feathers.Orders.GetAllMyOrders;

public class GetAllMyOrdersQueryHandler(IUnitOfWork unitOfWork) : IRequestHandler<GetAllMyOrdersQuery, IPaginatedList<OrderResponse>>
{
    private readonly IUnitOfWork _unitOfWork = unitOfWork;

    public async Task<IPaginatedList<OrderResponse>> Handle(GetAllMyOrdersQuery request, CancellationToken cancellationToken = default)
    {
        int year = request.Year;

        if (year == 0)
            year = DateTime.Now.Year;
        else if (year < 2026)
            return EmptyPaginatedList.Create<OrderResponse>();

        var orders = await _unitOfWork.Orders
            .FindPaginatedListAsync<OrderResponse>
            (
                x => x.CustomerId == request.UserId && x.Date.Year == year,
                request.Filters.PageNumber,
                request.Filters.PageSize,
                nameof(Order.Id),
                [nameof(Order.OrderItems)],
                cancellationToken
            );

        return orders;
    }
}
