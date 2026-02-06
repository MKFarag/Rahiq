namespace Application.Feathers.Carts.GetMyCart;

public class GetMyCartQueryHandler(IUnitOfWork unitOfWork, ICacheService cache) : IRequestHandler<GetMyCartQuery, CartResponse>
{
    private readonly IUnitOfWork _unitOfWork = unitOfWork;
    private readonly ICacheService _cache = cache;

    public async Task<CartResponse> Handle(GetMyCartQuery request, CancellationToken cancellationToken = default)
    {
        var cart = await _cache
            .GetOrCreateAsync
            (
                Cache.Keys.Cart(request.UserId),
                async token => await _unitOfWork.Carts
                .FindAllAsync
                (
                    x => x.CustomerId == request.UserId,
                    [nameof(Cart.Product), nameof(Cart.Bundle)],
                    cancellationToken
                ),
                Cache.Expirations.Medium,
                [Cache.Tags.Cart],
                cancellationToken
            );

        if (!cart.Any())
            return new CartResponse([], [], 0);

        cart = cart.OrderByDescending(x => x.AddedAt);

        var products = cart
            .Where(x => x.IsProduct && x.Product is not null)
            .Select(x => new CartProductResponse(
                x.ProductId!.Value,
                x.Product!.Name,
                x.UnitPrice,
                x.Quantity,
                x.Product.ImageUrl
            ));

        var bundles = cart
            .Where(x => x.IsBundle && x.Bundle is not null)
            .Select(x => new CartBundleResponse
            (
                x.BundleId!.Value,
                x.Bundle!.Name,
                x.UnitPrice,
                x.Quantity,
                x.Bundle.ImageUrl
            ));

        var totalPrice = cart.Sum(x => x.TotalPrice);
        return new CartResponse(products, bundles, totalPrice);
    }
}
