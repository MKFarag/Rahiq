namespace Application.Feathers.Types.DeleteType;

public class DeleteTypeCommandHandler(IUnitOfWork unitOfWork, ICacheService cache) : IRequestHandler<DeleteTypeCommand, Result>
{
    private readonly IUnitOfWork _unitOfWork = unitOfWork;
    private readonly ICacheService _cache = cache;

    public async Task<Result> Handle(DeleteTypeCommand request, CancellationToken cancellationToken = default)
    {
        if (await _unitOfWork.Types.GetAsync([request.Id], cancellationToken) is not { } type)
            return Result.Failure(TypeErrors.NotFound);

        if (await _unitOfWork.Products.AnyAsync(x => x.TypeId == request.Id, cancellationToken))
            return Result.Failure(TypeErrors.InUse);

        _unitOfWork.Types.Delete(type);

        await _unitOfWork.CompleteAsync(cancellationToken);

        await _cache.RemoveByTagAsync(Cache.Tags.Type, cancellationToken);

        return Result.Success();
    }
}
