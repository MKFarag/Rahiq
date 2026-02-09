namespace Application.Feathers.Products.AddProduct;

public class AddProductCommandHandler(IUnitOfWork unitOfWork, IFileStorageService fileStorageService, ICacheService cache) : IRequestHandler<AddProductCommand, Result<ProductResponse>>
{
    private readonly IUnitOfWork _unitOfWork = unitOfWork;
    private readonly IFileStorageService _fileStorageService = fileStorageService;
    private readonly ICacheService _cache = cache;

    public async Task<Result<ProductResponse>> Handle(AddProductCommand command, CancellationToken cancellationToken = default)
    {
        if (await _unitOfWork.Products.AnyAsync(x => x.Name == command.Request.Name, cancellationToken))
            return Result.Failure<ProductResponse>(ProductErrors.DuplicatedName);

        var product = command.Request.Adapt<Product>();

        await _unitOfWork.Products.AddAsync(product, cancellationToken);

        await _unitOfWork.CompleteAsync(cancellationToken);

        if (command.Image is not null)
        {
            var fileName = $"{product.Id}-{Path.GetFileName(command.Image.FileName)}.{Path.GetExtension(command.Image.FileName)}";

            await _fileStorageService.SaveAsync(command.Image.Stream, fileName, cancellationToken);

            var relativePath = _fileStorageService.GetRelativePath(fileName);

            product.ImageUrl = relativePath;

            await _unitOfWork.CompleteAsync(cancellationToken);
        }

        await _cache.RemoveByTagAsync(Cache.Tags.Product, cancellationToken);

        return Result.Success(product.Adapt<ProductResponse>());
    }
}
