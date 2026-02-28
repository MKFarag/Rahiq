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
                [nameof(Cart.Product), $"{nameof(Cart.Bundle)}.{nameof(Bundle.BundleItems)}.{nameof(BundleItem.Product)}"],
                cancellationToken
            );

        if (cart is null || !cart.Any())
            return Result.Failure<OrderResponse>(CartErrors.Empty);

        var order = new Order
        {
            CustomerId = request.UserId,
            Date = DateTime.UtcNow,
            Status = OrderStatus.Pending,
            Total = cart.Sum(x => x.TotalPrice),
            OrderItems = []
        };

        foreach (var item in cart)
        {
            if (item.IsProduct)
                order.OrderItems.Add(new OrderItem
                {
                    ProductId = item.ProductId!.Value,
                    Quantity = item.Quantity,
                    UnitPrice = item.UnitPrice
                });
            else
            {
                order.OrderItems.Add(new OrderItem
                {
                    BundleId = item.BundleId!.Value,
                    Quantity = item.Quantity,
                    UnitPrice = item.UnitPrice
                });

                if (!item.Bundle!.IsActive)
                    return Result.Failure<OrderResponse>(BundleErrors.NotActive);

                item.Bundle.QuantityAvailable -= item.Quantity;

                if (item.Bundle.QuantityAvailable < 0)
                    return Result.Failure<OrderResponse>(BundleErrors.InvalidQuantity);
            }
        }

        await _unitOfWork.Orders.AddAsync(order, cancellationToken);

        _unitOfWork.Carts.DeleteRange(cart);

        await _unitOfWork.CompleteAsync(cancellationToken);

        await _cache.RemoveAsync(Cache.Keys.Cart(request.UserId), cancellationToken);
        await _cache.RemoveByTagAsync([Cache.Tags.Bundle, Cache.Tags.Product], cancellationToken);

        return Result.Success(order.Adapt<OrderResponse>());
    }
}
