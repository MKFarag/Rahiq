namespace Application.Feathers.Bundles.AddBundleImage;

public class AddBundleImageCommandHandler(IUnitOfWork unitOfWork, IFileStorageService fileStorageService, ICacheService cache) : IRequestHandler<AddBundleImageCommand, Result>
{
    private readonly IUnitOfWork _unitOfWork = unitOfWork;
    private readonly IFileStorageService _fileStorageService = fileStorageService;
    private readonly ICacheService _cache = cache;

    public async Task<Result> Handle(AddBundleImageCommand request, CancellationToken cancellationToken = default)
    {
        if (await _unitOfWork.Bundles.GetAsync([request.Id], cancellationToken) is not { } bundle)
            return Result.Failure(BundleErrors.NotFound);

        if (bundle.ImageUrl is not null)
            return Result.Failure(BundleErrors.ImageExist);

        var fileName = $"{bundle.Id}-{Path.GetFileName(request.Image.FileName)}.{Path.GetExtension(request.Image.FileName)}";

        await _fileStorageService.SaveAsync(request.Image.Stream, fileName, cancellationToken);

        var relativePath = _fileStorageService.GetRelativePath(fileName);

        bundle.ImageUrl = relativePath;

        await _unitOfWork.CompleteAsync(cancellationToken);

        await _cache.RemoveByTagAsync(Cache.Tags.Bundle, cancellationToken);

        return Result.Success();
    }
}
