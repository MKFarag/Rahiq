namespace Application.Feathers.Bundles.ReactivateBundle;

public class ReactivateBundleCommandHandler(IUnitOfWork unitOfWork) : IRequestHandler<ReactivateBundleCommand, Result>
{
    private readonly IUnitOfWork _unitOfWork = unitOfWork;

    public async Task<Result> Handle(ReactivateBundleCommand request, CancellationToken cancellationToken = default)
    {
        if (await _unitOfWork.Bundles.GetAsync([request.Id], cancellationToken) is not { } bundle)
            return Result.Failure(BundleErrors.NotFound);

        if (bundle.IsActive)
            return Result.Failure(BundleErrors.IsActive);

        bundle.EndAt = request.Date;

        await _unitOfWork.CompleteAsync(cancellationToken);

        return Result.Success();
    }
}
