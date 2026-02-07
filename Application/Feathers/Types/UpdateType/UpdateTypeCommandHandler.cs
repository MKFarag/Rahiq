namespace Application.Feathers.Types.UpdateType;

public class UpdateTypeCommandHandler(IUnitOfWork unitOfWork, ICacheService cache) : IRequestHandler<UpdateTypeCommand, Result>
{
    private readonly IUnitOfWork _unitOfWork = unitOfWork;
    private readonly ICacheService _cache = cache;

    public async Task<Result> Handle(UpdateTypeCommand command, CancellationToken cancellationToken = default)
    {
        if (await _unitOfWork.Types.AnyAsync(t => t.Name == command.Request.Name && t.Id != command.Id, cancellationToken))
            return Result.Failure(TypeErrors.DuplicatedName);

        if (await _unitOfWork.Types.GetAsync([command.Id], cancellationToken) is not { } type)
            return Result.Failure(TypeErrors.NotFound);

        type.Name = command.Request.Name;

        await _unitOfWork.CompleteAsync(cancellationToken);

        await _cache.RemoveByTagAsync(Cache.Tags.Type, cancellationToken);

        return Result.Success();
    }
}
