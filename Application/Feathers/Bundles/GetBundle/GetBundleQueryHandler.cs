namespace Application.Feathers.Bundles.GetBundle;

public class GetBundleQueryHandler(IUnitOfWork unitOfWork) : IRequestHandler<GetBundleQuery, Result<BundleDetailResponse>>
{
    private readonly IUnitOfWork _unitOfWork = unitOfWork;

    public async Task<Result<BundleDetailResponse>> Handle(GetBundleQuery request, CancellationToken cancellationToken = default)
    {
        var bundle = await _unitOfWork.Bundles
            .FindAsync
            (
                x => x.Id == request.Id,
                [
                    $"{nameof(Bundle.BundleItems)}.{nameof(BundleItem.Product)}.{nameof(Product.Type)}",
                    $"{nameof(Bundle.BundleItems)}.{nameof(BundleItem.Product)}.{nameof(Product.Type)}"
                ],
                cancellationToken
            );

        return bundle is null
            ? Result.Failure<BundleDetailResponse>(BundleErrors.NotFound)
            : Result.Success(bundle.Adapt<BundleDetailResponse>());
    }
}
