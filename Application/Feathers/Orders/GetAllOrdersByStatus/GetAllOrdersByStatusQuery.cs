namespace Application.Feathers.Orders.GetAllOrdersByStatus;

public record GetAllOrdersByStatusQuery(SimpleRequestFilters Filters, string Status) : IRequest<IPaginatedList<OrderResponse>>;

