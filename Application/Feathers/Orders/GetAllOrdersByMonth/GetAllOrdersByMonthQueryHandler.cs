namespace Application.Feathers.Orders.GetAllOrdersByMonth;

public class GetAllOrdersByMonthQueryHandler(IUnitOfWork unitOfWork) : IRequestHandler<GetAllOrdersByMonthQuery, IPaginatedList<OrderResponse>>
{
    private readonly IUnitOfWork _unitOfWork = unitOfWork;

    public async Task<IPaginatedList<OrderResponse>> Handle(GetAllOrdersByMonthQuery request, CancellationToken cancellationToken = default)
    {
        var month = request.Month switch
        {
            0 => DateTime.Now.Month,
            > 12 or < 1 => -1,
            _ => request.Month
        };

        if (month == -1)
            return EmptyPaginatedList.Create<OrderResponse>();

        var orders = await _unitOfWork.Orders
            .FindPaginatedListAsync<OrderResponse>
            (
                x => x.Date.Month == month,
                request.Filters.PageNumber,
                request.Filters.PageSize,
                nameof(Order.Id),
                [nameof(Order.OrderItems)],
                cancellationToken
            );

        return orders;
    }
}
