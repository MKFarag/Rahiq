namespace Application.Feathers.Carts.RemoveCartItem;

public class RemoveCartItemCommandHandler(IUnitOfWork unitOfWork) : IRequestHandler<RemoveCartItemCommand, Result>
{
    private readonly IUnitOfWork _unitOfWork = unitOfWork;

    public async Task<Result> Handle(RemoveCartItemCommand request, CancellationToken cancellationToken = default)
    {
        if (!await _unitOfWork.Carts.AnyAsync(x => x.Id == request.CartId && x.CustomerId == request.UserId, cancellationToken))
            return Result.Failure(CartErrors.NotFound);
        
        await _unitOfWork.Carts.ExecuteDeleteAsync(x => x.Id == request.CartId, cancellationToken);

        return Result.Success();
    }
}
