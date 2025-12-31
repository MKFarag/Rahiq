namespace Application.Feathers.Products.ChangeProductStatus;

public class ChangeProductStatusCommandHandler(IUnitOfWork unitOfWork) : IRequestHandler<ChangeProductStatusCommand, Result>
{
    private readonly IUnitOfWork _unitOfWork = unitOfWork;

    public async Task<Result> Handle(ChangeProductStatusCommand request, CancellationToken cancellationToken = default)
    {
        if (await _unitOfWork.Products.GetAsync([request.Id], cancellationToken) is not { } product)
            return Result.Failure(ProductErrors.NotFound);

        product.IsAvailable = !product.IsAvailable;
        await _unitOfWork.CompleteAsync(cancellationToken);

        return Result.Success();
    }
}
