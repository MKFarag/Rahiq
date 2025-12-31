namespace Application.Feathers.Products.GetProduct;

public class GetProductQueryHandler(IUnitOfWork unitOfWork) : IRequestHandler<GetProductQuery, Result<ProductResponse>>
{
    private readonly IUnitOfWork _unitOfWork = unitOfWork;

    public async Task<Result<ProductResponse>> Handle(GetProductQuery request, CancellationToken cancellationToken = default)
    {
        var product = await _unitOfWork.Products.FindAsync
            (
                x => x.Id == request.Id,
                [nameof(Domain.Entities.Category), nameof(Domain.Entities.Type)],
                cancellationToken
            );

        return product is null
            ? Result.Failure<ProductResponse>(ProductErrors.NotFound)
            : Result.Success(product.Adapt<ProductResponse>());
    }
}
