namespace Application.Feathers.Carts.ClearMyCart;

public class ClearMyCartCommandHandler(IUnitOfWork unitOfWork, ICacheService cache) : IRequestHandler<ClearMyCartCommand, Result>
{
    private readonly IUnitOfWork _unitOfWork = unitOfWork;
    private readonly ICacheService _cache = cache;

    public async Task<Result> Handle(ClearMyCartCommand request, CancellationToken cancellationToken = default)
    {
        await _unitOfWork.Carts.ExecuteDeleteAsync(x => x.CustomerId == request.UserId, cancellationToken);

        await _cache.RemoveAsync(Cache.Keys.Cart(request.UserId), cancellationToken);

        return Result.Success();
    }
}
