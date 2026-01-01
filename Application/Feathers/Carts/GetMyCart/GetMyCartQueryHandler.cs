namespace Application.Feathers.Carts.GetMyCart;

public class GetMyCartQueryHandler(IUnitOfWork unitOfWork) : IRequestHandler<GetMyCartQuery, CartResponse>
{
    private readonly IUnitOfWork _unitOfWork = unitOfWork;

    public async Task<CartResponse> Handle(GetMyCartQuery request, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }
}
