using Application.Interfaces;

namespace Application.Feathers.Bundles.UpdateBundle;

public class UpdateBundleCommandHandler(IUnitOfWork unitOfWork) : IRequestHandler<UpdateBundleCommand, Result>
{
    private readonly IUnitOfWork _unitOfWork = unitOfWork;

    public async Task<Result> Handle(UpdateBundleCommand command, CancellationToken cancellationToken = default)
    {
        var bundle = await _unitOfWork.Bundles
            .TrackedFindAsync
            (
                b => b.Id == command.Id,
                [nameof(Bundle.BundleItems)],
                cancellationToken
            );

        if (bundle is null)
            return Result.Failure(BundleErrors.NotFound);

        bundle = command.Request.Adapt(bundle);

        var usedProductsId = bundle.BundleItems.Select(x => x.ProductId);

        if (command.Request.ProductsId.Except(usedProductsId).Any() || usedProductsId.Except(command.Request.ProductsId).Any())
        {
            var allowedProductsId = await _unitOfWork.Products.FindAllProjectionAsync(x => x.IsAvailable, x => x.Id, true, cancellationToken);

            if (!allowedProductsId.Any() || command.Request.ProductsId.Except(allowedProductsId).Any())
                return Result.Failure(ProductErrors.NotFound);
            
            await _unitOfWork.BundleItems.ExecuteDeleteAsync(x => x.BundleId == command.Id, cancellationToken);

            foreach (var id in command.Request.ProductsId)
                bundle.BundleItems.Add(new BundleItem { ProductId = id });
        }

        await _unitOfWork.CompleteAsync(cancellationToken);

        return Result.Success(cancellationToken);
    }
}