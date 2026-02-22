namespace Application.Feathers.Carts.UpdateCart;

public class UpdateCartCommandHandler(IUnitOfWork unitOfWork, ICacheService cache) : IRequestHandler<UpdateCartCommand, Result>
{
    private readonly IUnitOfWork _unitOfWork = unitOfWork;
    private readonly ICacheService _cache = cache;

    public async Task<Result> Handle(UpdateCartCommand request, CancellationToken cancellationToken = default)
    {
        if (await _unitOfWork.Carts.GetAsync([request.CartId], cancellationToken) is not { } cart)
            return Result.Failure(CartErrors.NotFound);

        if (cart.IsProduct)
        {
            var product = await _unitOfWork.Products.GetAsync([cart.ProductId!], cancellationToken);

            if (!product!.IsAvailable)
            {
                await _unitOfWork.Carts.ExecuteDeleteAsync(x => x.Id == cart.Id, cancellationToken);

                return Result.Failure(ProductErrors.NotAvailable);
            }
        }
        else
        {
            var bundle = await _unitOfWork.Bundles.GetAsync([cart.BundleId!], cancellationToken);

            if (request.Quantity > bundle!.QuantityAvailable)
                return Result.Failure(BundleErrors.InvalidQuantity);

            if (!bundle.IsActive)
            {
                await _unitOfWork.Carts.ExecuteDeleteAsync(x => x.Id == cart.Id, cancellationToken);

                return Result.Failure(BundleErrors.NotActive);
            }
        }

        await _unitOfWork.Carts
            .ExecuteUpdateAsync
            (
                x => x.Id == cart.Id,
                nameof(Cart.Quantity),
                request.Quantity,
                cancellationToken
            );

        await _cache.RemoveAsync(Cache.Keys.Cart(request.UserId), cancellationToken);

        return Result.Success();
    }
}
