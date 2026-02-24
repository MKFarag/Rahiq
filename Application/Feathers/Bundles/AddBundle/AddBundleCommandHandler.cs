namespace Application.Feathers.Bundles.AddBundle;

public class AddBundleCommandHandler(IUnitOfWork unitOfWork, IFileStorageService fileStorageService, ICacheService cache) : IRequestHandler<AddBundleCommand, Result<BundleDetailResponse>>
{
    private readonly IUnitOfWork _unitOfWork = unitOfWork;
    private readonly IFileStorageService _fileStorageService = fileStorageService;
    private readonly ICacheService _cache = cache;

    public async Task<Result<BundleDetailResponse>> Handle(AddBundleCommand command, CancellationToken cancellationToken = default)
    {
        if (await _unitOfWork.Bundles.AnyAsync(x => x.Name == command.Request.Name, cancellationToken))
            return Result.Failure<BundleDetailResponse>(BundleErrors.DuplicatedName);

        var allowedProductsId = await _unitOfWork.Products.FindAllProjectionAsync(x => x.IsAvailable, x => x.Id, true, cancellationToken);

        var requestedProductIds = command.Request.ProductsId.ToHashSet();

        if (!allowedProductsId.Any() || requestedProductIds.Except(allowedProductsId).Any())
            return Result.Failure<BundleDetailResponse>(ProductErrors.NotFound);

        var bundlesWithSameCountChecker = await _unitOfWork.Bundles
            .AnyWithWhereAsync
            (
                b => b.BundleItems.All(bi => requestedProductIds.Contains(bi.ProductId)),
                b => b.BundleItems.Count == requestedProductIds.Count,
                cancellationToken
            );

        if (bundlesWithSameCountChecker)
            return Result.Failure<BundleDetailResponse>(BundleErrors.DuplicatedProducts);

        var bundle = command.Request.Adapt<Bundle>();

        foreach (var id in requestedProductIds)
            bundle.BundleItems.Add(new BundleItem { ProductId = id });

        await _unitOfWork.Bundles.AddAsync(bundle, cancellationToken);

        await _unitOfWork.CompleteAsync(cancellationToken);

        if (command.Image is not null)
        {
            var fileName = $"{bundle.Id}-{Path.GetFileName(command.Image.FileName)}.{Path.GetExtension(command.Image.FileName)}";

            await _fileStorageService.SaveAsync(command.Image.Stream, fileName, cancellationToken);

            var relativePath = _fileStorageService.GetRelativePath(fileName);

            bundle.ImageUrl = relativePath;

            await _unitOfWork.CompleteAsync(cancellationToken);
        }

        await _cache.RemoveAsync([Cache.Keys.Bundles(true), Cache.Keys.Bundles(false)], cancellationToken);

        var response = await _unitOfWork.Bundles.FindAsync(
            x => x.Id == bundle.Id,
            [
                $"{nameof(Bundle.BundleItems)}.{nameof(BundleItem.Product)}.{nameof(Product.Type)}",
                $"{nameof(Bundle.BundleItems)}.{nameof(BundleItem.Product)}.{nameof(Product.Category)}"
            ],
            cancellationToken);

        return Result.Success(response!.Adapt<BundleDetailResponse>());
    }
}
