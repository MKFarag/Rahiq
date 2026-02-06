namespace Application.Feathers.Bundles.GetBundle;

public class GetBundleQueryHandler(IUnitOfWork unitOfWork, ICacheService cache) : IRequestHandler<GetBundleQuery, Result<BundleDetailResponse>>
{
    private readonly IUnitOfWork _unitOfWork = unitOfWork;
    private readonly ICacheService _cache = cache;

    public async Task<Result<BundleDetailResponse>> Handle(GetBundleQuery request, CancellationToken cancellationToken = default)
    {
        var bundle = await _cache
            .GetOrCreateAsync
            (
                Cache.Keys.Bundle(request.Id),
                async token => await _unitOfWork.Bundles
                .FindAsync
                (
                    x => x.Id == request.Id,
                    [
                        $"{nameof(Bundle.BundleItems)}.{nameof(BundleItem.Product)}.{nameof(Product.Type)}",
                        $"{nameof(Bundle.BundleItems)}.{nameof(BundleItem.Product)}.{nameof(Product.Category)}"
                    ],
                    token
                ),
                Cache.Expirations.Long,
                [Cache.Tags.Bundle],
                cancellationToken
            );

        return bundle is null
            ? Result.Failure<BundleDetailResponse>(BundleErrors.NotFound)
            : Result.Success(bundle.Adapt<BundleDetailResponse>());
    }
}
