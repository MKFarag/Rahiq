namespace Application.Feathers.Orders.AddOrder;

public class AddOrderCommandHandler(IUnitOfWork unitOfWork) : IRequestHandler<AddOrderCommand, Result<OrderResponse>>
{
    private readonly IUnitOfWork _unitOfWork = unitOfWork;

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

        foreach (var item in order.OrderItems)
        {
            if (item.IsProduct)
                continue;

            var bundle = await _unitOfWork.Bundles.GetAsync([item.BundleId!], cancellationToken);

            if (!bundle!.IsActive)
                return Result.Failure<OrderResponse>(BundleErrors.NotActive);

            bundle!.QuantityAvailable -= item.Quantity;

            if (bundle.QuantityAvailable < 0)
                return Result.Failure<OrderResponse>(BundleErrors.InvalidQuantity);
        }

        await _unitOfWork.Orders.AddAsync(order, cancellationToken);

        _unitOfWork.Carts.DeleteRange(cart);

        await _unitOfWork.CompleteAsync(cancellationToken);

        return Result.Success(order.Adapt<OrderResponse>());
    }
}
