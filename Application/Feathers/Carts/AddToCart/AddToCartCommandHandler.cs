namespace Application.Feathers.Carts.AddToCart;

public class AddToCartCommandHandler(IUnitOfWork unitOfWork) : IRequestHandler<AddToCartCommand, Result>
{
    private readonly IUnitOfWork _unitOfWork = unitOfWork;

    public async Task<Result> Handle(AddToCartCommand request, CancellationToken cancellationToken = default)
    {
        if (request.IsBundle)
        {
            if (await _unitOfWork.Bundles.GetAsync([request.ItemId], cancellationToken) is not { } bundle)
                return Result.Failure(BundleErrors.NotFound);

            if (request.Quantity > bundle.QuantityAvailable)
                return Result.Failure(BundleErrors.InvalidQuantity);

            if (!bundle.IsActive)
                return Result.Failure(BundleErrors.NotActive);

            var cart = await _unitOfWork.Carts
                .TrackedFindAllAsync(x => x.CustomerId == request.UserId && x.BundleId == request.ItemId, cancellationToken);

            if (cart.FirstOrDefault() is { } existingItem)
            {
                existingItem.Quantity += request.Quantity;

                if (existingItem.Quantity > bundle.QuantityAvailable)
                    return Result.Failure(BundleErrors.InvalidQuantity);
            }
            else
                await _unitOfWork.Carts.AddAsync(new Cart
                {
                    CustomerId = request.UserId,
                    BundleId = request.ItemId,
                    Quantity = request.Quantity
                }, cancellationToken);
        }
        else
        {
            if (await _unitOfWork.Products.GetAsync([request.ItemId], cancellationToken) is not { } product)
                return Result.Failure(ProductErrors.NotFound);

            if (!product.IsAvailable)
                return Result.Failure(ProductErrors.NotAvailable);

            var cart = await _unitOfWork.Carts
                .TrackedFindAllAsync(x => x.CustomerId == request.UserId && x.ProductId == request.ItemId, cancellationToken);

            if (cart.FirstOrDefault() is { } existingItem)
                existingItem.Quantity += request.Quantity;
            else
                await _unitOfWork.Carts.AddAsync(new Cart
                {
                    CustomerId = request.UserId,
                    ProductId = request.ItemId,
                    Quantity = request.Quantity
                }, cancellationToken);
        }

        await _unitOfWork.CompleteAsync(cancellationToken);

        return Result.Success();
    }
}