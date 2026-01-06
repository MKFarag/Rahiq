namespace Application.Feathers.Types.GetType;

public class GetTypeQueryHandler(IUnitOfWork unitOfWork) : IRequestHandler<GetTypeQuery, Result<TypeResponse>>
{
    private readonly IUnitOfWork _unitOfWork = unitOfWork;

    public async Task<Result<TypeResponse>> Handle(GetTypeQuery request, CancellationToken cancellationToken = default)
    {
        if (await _unitOfWork.Types.GetAsync([request.Id], cancellationToken) is not { } type)
            return Result.Failure<TypeResponse>(TypeErrors.NotFound);

        return Result.Success(type.Adapt<TypeResponse>());
    }
}
