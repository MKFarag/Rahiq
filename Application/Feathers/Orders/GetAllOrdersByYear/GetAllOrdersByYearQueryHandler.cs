namespace Application.Feathers.Orders.GetAllOrdersByYear;

public class GetAllOrdersByYearQueryHandler(IUnitOfWork unitOfWork) : IRequestHandler<GetAllOrdersByYearQuery, IPaginatedList<OrderResponse>>
{
    private readonly IUnitOfWork _unitOfWork = unitOfWork;

    public async Task<IPaginatedList<OrderResponse>> Handle(GetAllOrdersByYearQuery request, CancellationToken cancellationToken = default)
    {
        var year = request.Year switch
        {
            0 => DateTime.Now.Year,
            _ => request.Year
        };

        if (year < 2026)
            return EmptyPaginatedList.Create<OrderResponse>();

        var orders = await _unitOfWork.Orders
            .FindPaginatedListAsync<OrderResponse>
            (
                x => x.Date.Year == year,
                request.Filters.PageNumber,
                request.Filters.PageSize,
                nameof(Order.Id),
                [nameof(Order.OrderItems)],
                cancellationToken
            );

        return orders;
    }
}
