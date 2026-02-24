namespace Application.Feathers.Carts.UpdateCart;

public class UpdateCartCommandHandler(IUnitOfWork unitOfWork, ICacheService cache) : IRequestHandler<UpdateCartCommand, Result>
{
    private readonly IUnitOfWork _unitOfWork = unitOfWork;
    private readonly ICacheService _cache = cache;

    public async Task<Result> Handle(UpdateCartCommand request, CancellationToken cancellationToken = default)
    {
        if (await _unitOfWork.Carts.GetAsync([request.CartId], cancellationToken) is not { } cart)
            return Result.Failure(CartErrors.NotFound);

        if (cart.CustomerId != request.UserId)
            return Result.Failure(CartErrors.Forbidden);

        Result result = Result.Success();

        if (cart.IsProduct)
        {
            var product = await _unitOfWork.Products.GetAsync([cart.ProductId!], cancellationToken);

            if (!product!.IsAvailable)
                result = Result.Failure(ProductErrors.NotAvailable);
        }
        else
        {
            var bundle = await _unitOfWork.Bundles.GetAsync([cart.BundleId!], cancellationToken);

            if (request.Quantity > bundle!.QuantityAvailable)
                return Result.Failure(BundleErrors.InvalidQuantity);

            if (!bundle.IsActive)
                result = Result.Failure(BundleErrors.NotActive);
        }

        if (result.IsSuccess)
            await _unitOfWork.Carts
                .ExecuteUpdateAsync
                (
                    x => x.Id == cart.Id,
                    nameof(Cart.Quantity),
                    request.Quantity,
                    cancellationToken
                );
        else
            await _unitOfWork.Carts.ExecuteDeleteAsync(x => x.Id == cart.Id, cancellationToken);

        await _cache.RemoveAsync(Cache.Keys.Cart(request.UserId), cancellationToken);

        return result;
    }
}
