namespace Application.Feathers.Products.AddProductDiscount;

public class AddProductDiscountCommandHandler(IUnitOfWork unitOfWork) : IRequestHandler<AddProductDiscountCommand, Result>
{
    private readonly IUnitOfWork _unitOfWork = unitOfWork;

    public async Task<Result> Handle(AddProductDiscountCommand request, CancellationToken cancellationToken = default)
    {
        if (request.Value is > 100 or < 0)
            return Result.Failure(ProductErrors.Percentage);

        if (await _unitOfWork.Products.GetAsync([request.Id], cancellationToken) is not { } product)
            return Result.Failure(ProductErrors.NotFound);

        product.DiscountPercentage = request.Value;

        await _unitOfWork.CompleteAsync(cancellationToken);

        return Result.Success();
    }
}
