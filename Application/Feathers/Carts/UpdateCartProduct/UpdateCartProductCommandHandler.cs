namespace Application.Feathers.Carts.UpdateCartProduct;

public class UpdateCartProductCommandHandler(IUnitOfWork unitOfWork, ICacheService cache) : IRequestHandler<UpdateCartProductCommand, Result>
{
    private readonly IUnitOfWork _unitOfWork = unitOfWork;
    private readonly ICacheService _cache = cache;

    public async Task<Result> Handle(UpdateCartProductCommand request, CancellationToken cancellationToken = default)
    {
        if (await _unitOfWork.Products.GetAsync([request.ProductId], cancellationToken) is not { } product)
            return Result.Failure(ProductErrors.NotFound);

        if (!product.IsAvailable)
            return Result.Failure(ProductErrors.NotAvailable);

        var cart = await _unitOfWork.Carts.GetAsync([request.ProductId, request.UserId], cancellationToken);

        if (cart is null)
        {
            if (request.Quantity == 0)
                return Result.Failure(CartErrors.InvalidQuantity);

            await _unitOfWork.Carts.AddAsync(new Cart
            {
                CustomerId = request.UserId,
                ProductId = request.ProductId,
                Quantity = request.Quantity
            },
            cancellationToken);

            await _unitOfWork.CompleteAsync(cancellationToken);
        }
        else
        {
            if (request.Quantity > 0)
                await _unitOfWork.Carts
                    .ExecuteUpdateAsync
                    (
                        x => x.ProductId == request.ProductId && x.CustomerId == request.UserId,
                        nameof(Cart.Quantity), request.Quantity,
                        cancellationToken
                    );
            else if (request.Quantity == 0)
                await _unitOfWork.Carts.ExecuteDeleteAsync(x => x.ProductId == request.ProductId && x.CustomerId == request.UserId, cancellationToken);
            else
                return Result.Failure(CartErrors.InvalidQuantity);
        }

        await _cache.RemoveAsync(Cache.Keys.Cart(request.UserId), cancellationToken);

        return Result.Success();
    }
}
