//namespace Application.Feathers.Orders.AddOrder;

//public class AddOrderCommandHandler(IUnitOfWork unitOfWork) : IRequestHandler<AddOrderCommand, Result>
//{
//    private readonly IUnitOfWork _unitOfWork = unitOfWork;

//    public async Task<Result> Handle(AddOrderCommand request, CancellationToken cancellationToken = default)
//    {
//        var cart = await _unitOfWork.Carts.FindAllAsync(x => x.CustomerId == request.UserId, cancellationToken);

//        if (!cart.Any() || cart is null)
//            return Result.Failure(CartErrors.Empty);
//    }
//}
