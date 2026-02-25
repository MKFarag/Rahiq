namespace Application.Feathers.Orders.AddOrder;

public class AddOrderCommandHandler(IUnitOfWork unitOfWork, ICacheService cache) : IRequestHandler<AddOrderCommand, Result<OrderResponse>>
{
    private readonly IUnitOfWork _unitOfWork = unitOfWork;
    private readonly ICacheService _cache = cache;

    public async Task<Result<OrderResponse>> Handle(AddOrderCommand request, CancellationToken cancellationToken = default)
    {
        var cart = await _unitOfWork.Carts
            .FindAllAsync
            (
                x => x.CustomerId == request.UserId,
                [nameof(Cart.Product), nameof(Cart.Bundle)],
                cancellationToken
            );

        if (cart is null || !cart.Any())
            return Result.Failure<OrderResponse>(CartErrors.Empty);

        var order = new Order
        {
            CustomerId = request.UserId,
            Status = OrderStatus.Pending,
            Total = cart.Sum(x => x.TotalPrice),
            OrderItems = [.. cart.Select(x => new OrderItem
            {
                ProductId = x.IsProduct ? x.ProductId : null,
                BundleId = x.IsBundle ? x.BundleId : null,
                Quantity = x.Quantity,
                UnitPrice = x.UnitPrice
            })],
        };

        var bundlesInCart = cart
            .Where(x => x.IsBundle)
            .GroupBy(x => x.BundleId)
            .Select(g => new { Bundle = g.First().Bundle!, TotalQuantity = g.Sum(x => x.Quantity) });

        foreach (var entry in bundlesInCart)
        {
            if (!entry.Bundle.IsActive)
                return Result.Failure<OrderResponse>(BundleErrors.NotActive);

            entry.Bundle.QuantityAvailable -= entry.TotalQuantity;

            if (entry.Bundle.QuantityAvailable < 0)
                return Result.Failure<OrderResponse>(BundleErrors.InvalidQuantity);
        }

        await _unitOfWork.Orders.AddAsync(order, cancellationToken);

        _unitOfWork.Carts.DeleteRange(cart);

        await _unitOfWork.CompleteAsync(cancellationToken);

        await _cache.RemoveAsync(Cache.Keys.Cart(request.UserId), cancellationToken);
        await _cache.RemoveByTagAsync([Cache.Tags.Bundle, Cache.Tags.Product], cancellationToken);

        return Result.Success(order.Adapt<OrderResponse>());
    }
}
