namespace Application.Feathers.Types.GetAllTypes;

public class GetAllTypesQueryHandler(IUnitOfWork unitOfWork) : IRequestHandler<GetAllTypesQuery, IEnumerable<TypeResponse>>
{
    private readonly IUnitOfWork _unitOfWork = unitOfWork;

    public async Task<IEnumerable<TypeResponse>> Handle(GetAllTypesQuery request, CancellationToken cancellationToken = default)
        => await _unitOfWork.Types.GetAllProjectionAsync<TypeResponse>(cancellationToken);
}
