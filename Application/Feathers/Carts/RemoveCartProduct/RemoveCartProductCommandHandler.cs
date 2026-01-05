namespace Application.Feathers.Carts.RemoveCartProduct;

public class RemoveCartProductCommandHandler(IUnitOfWork unitOfWork) : IRequestHandler<RemoveCartProductCommand, Result>
{
    private readonly IUnitOfWork _unitOfWork = unitOfWork;

    public async Task<Result> Handle(RemoveCartProductCommand request, CancellationToken cancellationToken = default)
    {
        await _unitOfWork.Carts.ExecuteDeleteAsync(x => x.CustomerId == request.UserId && x.ProductId == request.ProductId, cancellationToken);

        return Result.Success();
    }
}
