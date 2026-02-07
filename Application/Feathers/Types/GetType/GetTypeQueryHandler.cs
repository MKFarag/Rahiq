namespace Application.Feathers.Types.GetType;

public class GetTypeQueryHandler(IUnitOfWork unitOfWork, ICacheService cache) : IRequestHandler<GetTypeQuery, Result<TypeResponse>>
{
    private readonly IUnitOfWork _unitOfWork = unitOfWork;
    private readonly ICacheService _cache = cache;

    public async Task<Result<TypeResponse>> Handle(GetTypeQuery request, CancellationToken cancellationToken = default)
    {
        var type = await _cache
            .GetOrCreateAsync
            (
                Cache.Keys.Type(request.Id),
                async token => await _unitOfWork.Types.GetAsync([request.Id], token),
                Cache.Expirations.VeryLong,
                [Cache.Tags.Type],
                cancellationToken
            );

        if (type is not null)
            return Result.Success(type.Adapt<TypeResponse>());

        await _cache.RemoveAsync(Cache.Keys.Type(request.Id), cancellationToken);

        return Result.Failure<TypeResponse>(TypeErrors.NotFound);
    }
}
