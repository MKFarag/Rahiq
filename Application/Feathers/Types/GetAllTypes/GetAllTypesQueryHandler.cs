namespace Application.Feathers.Types.GetAllTypes;

public class GetAllTypesQueryHandler(IUnitOfWork unitOfWork, ICacheService cache) : IRequestHandler<GetAllTypesQuery, IEnumerable<TypeResponse>>
{
    private readonly IUnitOfWork _unitOfWork = unitOfWork;
    private readonly ICacheService _cache = cache;

    public async Task<IEnumerable<TypeResponse>> Handle(GetAllTypesQuery request, CancellationToken cancellationToken = default)
        => await _cache
        .GetOrCreateAsync
        (
            Cache.Keys.Types,
            async token => await _unitOfWork.Types.GetAllProjectionAsync<TypeResponse>(cancellationToken),
            null,
            [Cache.Tags.Type],
            cancellationToken
        );
}
