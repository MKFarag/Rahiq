namespace Application.Feathers.Orders.GetAllOrdersByMonth;

public record GetAllOrdersByMonthQuery(SimpleRequestFilters Filters, int Month) : IRequest<IPaginatedList<OrderResponse>>;

