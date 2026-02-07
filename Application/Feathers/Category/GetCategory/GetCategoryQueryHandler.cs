namespace Application.Feathers.Category.GetCategory;

public class GetCategoryQueryHandler(IUnitOfWork unitOfWork, ICacheService cache) : IRequestHandler<GetCategoryQuery, Result<CategoryResponse>>
{
    private readonly IUnitOfWork _unitOfWork = unitOfWork;
    private readonly ICacheService _cache = cache;

    public async Task<Result<CategoryResponse>> Handle(GetCategoryQuery request, CancellationToken cancellationToken = default)
    {
        var category = await _cache
        .GetOrCreateAsync
        (
            Cache.Keys.Category(request.Id),
            async token => await _unitOfWork.Categories.GetAsync([request.Id], token),
            Cache.Expirations.VeryLong,
            [Cache.Tags.Category],
            cancellationToken
        );

        if (category is not null)
            return Result.Success(category.Adapt<CategoryResponse>());

        await _cache.RemoveAsync(Cache.Keys.Category(request.Id), cancellationToken);

        return Result.Failure<CategoryResponse>(CategoryErrors.NotFound);
    }
}
