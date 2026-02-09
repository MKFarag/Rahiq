namespace Application.Feathers.Products.AddProductImage;

public class AddProductImageCommandHandler(IUnitOfWork unitOfWork, IFileStorageService fileStorageService, ICacheService cache) : IRequestHandler<AddProductImageCommand, Result>
{
    private readonly IUnitOfWork _unitOfWork = unitOfWork;
    private readonly IFileStorageService _fileStorageService = fileStorageService;
    private readonly ICacheService _cache = cache;

    public async Task<Result> Handle(AddProductImageCommand request, CancellationToken cancellationToken = default)
    {
        if (await _unitOfWork.Products.GetAsync([request.Id], cancellationToken) is not { } product)
            return Result.Failure(ProductErrors.NotFound);

        if (product.ImageUrl is not null)
            return Result.Failure(ProductErrors.ImageExist);

        var fileName = $"{product.Id}-{Path.GetFileName(request.Image.FileName)}.{Path.GetExtension(request.Image.FileName)}";

        await _fileStorageService.SaveAsync(request.Image.Stream, fileName, cancellationToken);

        var relativePath = _fileStorageService.GetRelativePath(fileName);

        product.ImageUrl = relativePath;

        await _unitOfWork.CompleteAsync(cancellationToken);

        await _cache.RemoveByTagAsync(Cache.Tags.Product, cancellationToken);

        return Result.Success();
    }
}
