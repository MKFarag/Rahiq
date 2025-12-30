namespace Application.Feathers.Product.GetAllProducts;

public record GetAllProductsQuery(RequestFilters Filters, bool IncludeNotAvailable) : IRequest<IPaginatedList<ProductResponse>>;
