namespace Application.Feathers.Products.UpdateProduct;

public class UpdateProductCommandHandler(IUnitOfWork unitOfWork) : IRequestHandler<UpdateProductCommand, Result>
{
    private readonly IUnitOfWork _unitOfWork = unitOfWork;

    public async Task<Result> Handle(UpdateProductCommand command, CancellationToken cancellationToken = default)
    {
        if (await _unitOfWork.Products.GetAsync([command.Id], cancellationToken) is not { } product)
            return Result.Failure(ProductErrors.NotFound);

        if (!await _unitOfWork.Categories.AnyAsync(x => x.Id == command.Request.CategoryId, cancellationToken))
            return Result.Failure(CategoryErrors.NotFound);

        if (!await _unitOfWork.Types.AnyAsync(x => x.Id == command.Request.TypeId, cancellationToken))
            return Result.Failure(CategoryErrors.NotFound);

        product = command.Request.Adapt(product);

        await _unitOfWork.CompleteAsync(cancellationToken);

        return Result.Success();
    }
}
