namespace Application.Feathers.Products.GetAllProducts;

public record GetAllProductsQuery(RequestFilters Filters, bool IncludeNotAvailable) : IRequest<IPaginatedList<ProductResponse>>;
