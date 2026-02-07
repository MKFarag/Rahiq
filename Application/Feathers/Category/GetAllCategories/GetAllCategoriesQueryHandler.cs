namespace Application.Feathers.Category.GetAllCategories;

public class GetAllCategoriesQueryHandler(IUnitOfWork unitOfWork, ICacheService cache) : IRequestHandler<GetAllCategoriesQuery, IEnumerable<CategoryResponse>>
{
    private readonly IUnitOfWork _unitOfWork = unitOfWork;
    private readonly ICacheService _cache = cache;

    public async Task<IEnumerable<CategoryResponse>> Handle(GetAllCategoriesQuery request, CancellationToken cancellationToken = default)
        => await _cache
        .GetOrCreateAsync
        (
            Cache.Keys.Categories,
            async token => await _unitOfWork.Categories.GetAllProjectionAsync<CategoryResponse>(token),
            null,
            [Cache.Tags.Category],
            cancellationToken
        );
}
