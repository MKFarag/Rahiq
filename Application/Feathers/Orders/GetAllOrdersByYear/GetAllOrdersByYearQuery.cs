namespace Application.Feathers.Orders.GetAllOrdersByYear;

public record GetAllOrdersByYearQuery(SimpleRequestFilters Filters, int Year) : IRequest<IPaginatedList<OrderResponse>>;

