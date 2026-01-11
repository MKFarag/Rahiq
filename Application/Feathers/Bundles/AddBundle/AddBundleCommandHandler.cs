namespace Application.Feathers.Bundles.AddBundle;

public class AddBundleCommandHandler(IUnitOfWork unitOfWork, IFileStorageService fileStorageService) : IRequestHandler<AddBundleCommand, Result<BundleDetailResponse>>
{
    private readonly IUnitOfWork _unitOfWork = unitOfWork;
    private readonly IFileStorageService _fileStorageService = fileStorageService;

    public async Task<Result<BundleDetailResponse>> Handle(AddBundleCommand command, CancellationToken cancellationToken = default)
    {
        if (await _unitOfWork.Bundles.AnyAsync(x => x.Name == command.Request.Name, cancellationToken))
            return Result.Failure<BundleDetailResponse>(BundleErrors.DuplicatedName);

        var allowedProductsId = await _unitOfWork.Products.FindAllProjectionAsync(x => x.IsAvailable, x => x.Id, true, cancellationToken);

        if (!allowedProductsId.Any() || command.Request.ProductsId.Except(allowedProductsId).Any())
            return Result.Failure<BundleDetailResponse>(ProductErrors.NotFound);

        var bundle = command.Request.Adapt<Bundle>();

        foreach (var id in command.Request.ProductsId)
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

        return Result.Success(bundle.Adapt<BundleDetailResponse>());
    }
}
