namespace Application.Feathers.Bundles.ReactivateBundle;

public class ReactivateBundleCommandHandler(IUnitOfWork unitOfWork, ICacheService cache) : IRequestHandler<ReactivateBundleCommand, Result>
{
    private readonly IUnitOfWork _unitOfWork = unitOfWork;
    private readonly ICacheService _cache = cache;

    public async Task<Result> Handle(ReactivateBundleCommand request, CancellationToken cancellationToken = default)
    {
        if (await _unitOfWork.Bundles.GetAsync([request.Id], cancellationToken) is not { } bundle)
            return Result.Failure(BundleErrors.NotFound);

        if (bundle.IsActive)
            return Result.Failure(BundleErrors.IsActive);

        bundle.EndAt = request.Date;

        await _unitOfWork.CompleteAsync(cancellationToken);

        await _cache.RemoveByTagAsync(Cache.Tags.Bundle, cancellationToken);

        return Result.Success();
    }
}
