namespace Application.Feathers.Carts.ClearMyCart;

public class ClearMyCartCommandHandler(IUnitOfWork unitOfWork) : IRequestHandler<ClearMyCartCommand, Result>
{
    private readonly IUnitOfWork _unitOfWork = unitOfWork;

    public async Task<Result> Handle(ClearMyCartCommand request, CancellationToken cancellationToken = default)
    {
        await _unitOfWork.Carts.ExecuteDeleteAsync(x => x.CustomerId == request.UserId, cancellationToken);

        return Result.Success();
    }
}
