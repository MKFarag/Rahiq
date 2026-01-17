namespace Application.Feathers.Orders.GetAllMyOrders;

public record GetAllMyOrdersQuery(SimpleRequestFilters Filters, string UserId, int Year) : IRequest<IPaginatedList<OrderResponse>>;

