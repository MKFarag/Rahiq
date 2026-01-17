namespace Application.Feathers.Carts.GetMyCart;

public class GetMyCartQueryHandler(IUnitOfWork unitOfWork) : IRequestHandler<GetMyCartQuery, CartResponse>
{
    private readonly IUnitOfWork _unitOfWork = unitOfWork;

    public async Task<CartResponse> Handle(GetMyCartQuery request, CancellationToken cancellationToken = default)
    {
        var carts = await _unitOfWork.Carts.FindAllAsync(
            x => x.CustomerId == request.UserId,
            [nameof(Cart.Product), nameof(Cart.Bundle)],
            cancellationToken
        );
        if (!carts.Any())
            return new CartResponse([], [], 0);

        carts = carts.OrderByDescending(x => x.AddedAt);

        var products = carts
            .Where(x => x.IsProduct && x.Product is not null)
            .Select(x => new CartProductResponse(
                x.ProductId!.Value,
                x.Product!.Name,
                x.UnitPrice,
                x.Quantity,
                x.Product.ImageUrl
            ));

        var bundles = carts
            .Where(x => x.IsBundle && x.Bundle is not null)
            .Select(x => new CartBundleResponse
            (
                x.BundleId!.Value,
                x.Bundle!.Name,
                x.UnitPrice,
                x.Quantity,
                x.Bundle.ImageUrl
            ));

        var totalPrice = carts.Sum(x => x.TotalPrice);
        return new CartResponse(products, bundles, totalPrice);
    }
}
