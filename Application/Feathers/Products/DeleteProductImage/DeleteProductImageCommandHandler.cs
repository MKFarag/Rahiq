namespace Application.Feathers.Products.DeleteProductImage;

public class DeleteProductImageCommandHandler(IUnitOfWork unitOfWork, IFileStorageService fileStorageService) : IRequestHandler<DeleteProductImageCommand, Result>
{
    private readonly IUnitOfWork _unitOfWork = unitOfWork;
    private readonly IFileStorageService _fileStorageService = fileStorageService;

    public async Task<Result> Handle(DeleteProductImageCommand request, CancellationToken cancellationToken = default)
    {
        if (await _unitOfWork.Products.GetAsync([request.Id], cancellationToken) is not { } product)
            return Result.Failure(ProductErrors.NotFound);

        if (string.IsNullOrEmpty(product.ImageUrl))
            return Result.Failure(ProductErrors.NoImage);

        await _fileStorageService.RemoveAsync(product.ImageUrl, cancellationToken);

        product.ImageUrl = null;

        return Result.Success();
    }
}
