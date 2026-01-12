namespace Application.Feathers.Bundles.DeactivateBundle;

public class DeactivateBundleCommandHandler(IUnitOfWork unitOfWork) : IRequestHandler<DeactivateBundleCommand, Result>
{
    private readonly IUnitOfWork _unitOfWork = unitOfWork;

    public async Task<Result> Handle(DeactivateBundleCommand request, CancellationToken cancellationToken = default)
    {
        if (await _unitOfWork.Bundles.GetAsync([request.Id], cancellationToken) is not { } bundle)
            return Result.Failure(BundleErrors.NotFound);

        bundle.EndAt = DateOnly.FromDateTime(DateTime.Today);

        await _unitOfWork.CompleteAsync(cancellationToken);

        return Result.Success();
    }
}
