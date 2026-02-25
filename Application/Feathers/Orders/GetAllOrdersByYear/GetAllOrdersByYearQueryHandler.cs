namespace Application.Feathers.Orders.GetAllOrdersByYear;

public class GetAllOrdersByYearQueryHandler(IUnitOfWork unitOfWork) : IRequestHandler<GetAllOrdersByYearQuery, IPaginatedList<OrderResponse>>
{
    private readonly IUnitOfWork _unitOfWork = unitOfWork;

    public async Task<IPaginatedList<OrderResponse>> Handle(GetAllOrdersByYearQuery request, CancellationToken cancellationToken = default)
    {
        if (request.Year < 2026)
            return EmptyPaginatedList.Create<OrderResponse>();

        var orders = await _unitOfWork.Orders
            .FindPaginatedListAsync<OrderResponse>
            (
                x => x.Date.Year == request.Year,
                request.Filters.PageNumber,
                request.Filters.PageSize,
                [nameof(Order.OrderItems)],
                cancellationToken
            );

        return orders;
    }
}
