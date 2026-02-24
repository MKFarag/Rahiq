namespace Application.Feathers.Bundles.GetAllBundles;

public class GetAllBundlesQueryHandler(IUnitOfWork unitOfWork, ICacheService cache) : IRequestHandler<GetAllBundlesQuery, IEnumerable<BundleResponse>>
{
    private readonly IUnitOfWork _unitOfWork = unitOfWork;
    private readonly ICacheService _cache = cache;

    public async Task<IEnumerable<BundleResponse>> Handle(GetAllBundlesQuery request, CancellationToken cancellationToken = default)
    {
        var bundles = await _cache
            .GetOrCreateAsync
            (
                Cache.Keys.Bundles(request.IncludeNotAvailable),
                async token => await _unitOfWork.Bundles
                .FindAllProjectionAsync<BundleResponse>
                (
                    x => request.IncludeNotAvailable || (x.EndAt > DateOnly.FromDateTime(DateTime.UtcNow) && x.QuantityAvailable > 0),
                    [$"{nameof(Bundle.BundleItems)}.{nameof(BundleItem.Product)}", QueryHint.SplitQuery],
                    token
                ),
                null,
                [Cache.Tags.Bundle],
                cancellationToken
            );

        return bundles;
    }
}
