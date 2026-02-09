namespace Application.Feathers.Products.GetProduct;

public class GetProductQueryHandler(IUnitOfWork unitOfWork, ICacheService cache) : IRequestHandler<GetProductQuery, Result<ProductResponse>>
{
    private readonly IUnitOfWork _unitOfWork = unitOfWork;
    private readonly ICacheService _cache = cache;

    public async Task<Result<ProductResponse>> Handle(GetProductQuery request, CancellationToken cancellationToken = default)
    {
        var product = await _cache
            .GetOrCreateAsync
            (
                Cache.Keys.Product(request.Id),
                async token => await _unitOfWork.Products.FindAsync
                (
                    x => x.Id == request.Id,
                    [nameof(Product.Category), nameof(Product.Type)],
                    token
                ),
                Cache.Expirations.Long,
                [Cache.Tags.Product],
                cancellationToken
            );

        return product is null
            ? Result.Failure<ProductResponse>(ProductErrors.NotFound)
            : Result.Success(product.Adapt<ProductResponse>());
    }
}
