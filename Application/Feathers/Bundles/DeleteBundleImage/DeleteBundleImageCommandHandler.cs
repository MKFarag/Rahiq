namespace Application.Feathers.Bundles.DeleteBundleImage;

public class DeleteBundleImageCommandHandler(IUnitOfWork unitOfWork, IFileStorageService fileStorageService) : IRequestHandler<DeleteBundleImageCommand, Result>
{
    private readonly IUnitOfWork _unitOfWork = unitOfWork;
    private readonly IFileStorageService _fileStorageService = fileStorageService;

    public async Task<Result> Handle(DeleteBundleImageCommand request, CancellationToken cancellationToken = default)
    {
        if (await _unitOfWork.Bundles.GetAsync([request.Id], cancellationToken) is not { } bundle)
            return Result.Failure(BundleErrors.NotFound);

        if (string.IsNullOrEmpty(bundle.ImageUrl))
            return Result.Failure(BundleErrors.NoImage);

        await _fileStorageService.RemoveAsync(bundle.ImageUrl, cancellationToken);

        bundle.ImageUrl = null;

        return Result.Success();
    }
}
