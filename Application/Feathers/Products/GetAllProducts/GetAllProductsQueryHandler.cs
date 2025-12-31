namespace Application.Feathers.Products.GetAllProducts;

public class GetAllProductsQueryHandler(IUnitOfWork unitOfWork) : IRequestHandler<GetAllProductsQuery, IPaginatedList<ProductResponse>>
{
    private readonly IUnitOfWork _unitOfWork = unitOfWork;
    private readonly HashSet<string> _allowedSearchColumns = new(StringComparer.OrdinalIgnoreCase) { "Name", "Brand" };
    private readonly HashSet<string> _allowedSortColumns = new(StringComparer.OrdinalIgnoreCase) { "Id", "Name", "Price" };

    public async Task<IPaginatedList<ProductResponse>> Handle(GetAllProductsQuery request, CancellationToken cancellationToken = default)
    {
        var filters = request.Filters.Check(_allowedSortColumns, _allowedSearchColumns);

        var response = await _unitOfWork.Products.FindPaginatedListAsync<ProductResponse>
            (
                x => (request.IncludeNotAvailable || x.IsAvailable),
                filters.PageNumber, filters.PageSize, filters.SearchValue, filters.SearchColumn, filters.SortColumn!, filters.SortDirection!, ColumnType.String,
                [nameof(Domain.Entities.Category), nameof(Domain.Entities.Type)],
                cancellationToken
            );

        return response;
    }       
}
