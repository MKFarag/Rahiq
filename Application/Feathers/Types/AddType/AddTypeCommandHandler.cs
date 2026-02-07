namespace Application.Feathers.Types.AddType;

public class AddTypeCommandHandler(IUnitOfWork unitOfWork, ICacheService cache) : IRequestHandler<AddTypeCommand, Result<TypeResponse>>
{
    private readonly IUnitOfWork _unitOfWork = unitOfWork;
    private readonly ICacheService _cache = cache;

    public async Task<Result<TypeResponse>> Handle(AddTypeCommand command, CancellationToken cancellationToken = default)
    {
        if (await _unitOfWork.Types.AnyAsync(x => x.Name == command.Request.Name, cancellationToken))
            return Result.Failure<TypeResponse>(TypeErrors.DuplicatedName);

        var type = new Domain.Entities.Type { Name = command.Request.Name };

        await _unitOfWork.Types.AddAsync(type, cancellationToken);

        await _unitOfWork.CompleteAsync(cancellationToken);

        await _cache.RemoveAsync(Cache.Keys.Types, cancellationToken);

        return Result.Success(type.Adapt<TypeResponse>());
    }
}
