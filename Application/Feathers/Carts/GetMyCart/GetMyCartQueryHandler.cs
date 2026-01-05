namespace Application.Feathers.Carts.GetMyCart;

public class GetMyCartQueryHandler(IUnitOfWork unitOfWork) : IRequestHandler<GetMyCartQuery, CartResponse>
{
    private readonly IUnitOfWork _unitOfWork = unitOfWork;

    public async Task<CartResponse> Handle(GetMyCartQuery request, CancellationToken cancellationToken = default)
    {
        var carts = await _unitOfWork.Carts.FindAllAsync(x => x.CustomerId == request.UserId, [nameof(Cart.Product)], cancellationToken);

        if (!carts.Any())
            return new CartResponse([], 0);

        var totalPrice = carts.Sum(x => x.Product.SellingPrice * x.Quantity);

        var products = carts.Select(x => new CartProductResponse(x.ProductId, x.Product.Name, x.Product.SellingPrice, x.Quantity, x.Product.ImageUrl));

        return new CartResponse(products, totalPrice);
    }
}
