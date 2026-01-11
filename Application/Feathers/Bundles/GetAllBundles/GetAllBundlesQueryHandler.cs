namespace Application.Feathers.Bundles.GetAllBundles;

public class GetAllBundlesQueryHandler(IUnitOfWork unitOfWork) : IRequestHandler<GetAllBundlesQuery, IEnumerable<BundleResponse>>
{
    private readonly IUnitOfWork _unitOfWork = unitOfWork;

    public async Task<IEnumerable<BundleResponse>> Handle(GetAllBundlesQuery request, CancellationToken cancellationToken = default)
        => await _unitOfWork.Bundles
            .FindAllProjectionAsync<BundleResponse>
            (
                x => request.IncludeNotAvailable || x.IsActive,
                [$"{nameof(Bundle.BundleItems)}.{nameof(BundleItem.Product)}"],
                cancellationToken
            );
}
