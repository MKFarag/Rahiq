namespace Application.Feathers.Category.GetCategory;

public class GetCategoryQueryHandler(IUnitOfWork unitOfWork) : IRequestHandler<GetCategoryQuery, Result<CategoryResponse>>
{
    private readonly IUnitOfWork _unitOfWork = unitOfWork;

    public async Task<Result<CategoryResponse>> Handle(GetCategoryQuery request, CancellationToken cancellationToken = default)
    {
        if (await _unitOfWork.Categories.GetAsync([request.Id], cancellationToken) is not { } category)
            return Result.Failure<CategoryResponse>(CategoryErrors.NotFound);

        return Result.Success(category.Adapt<CategoryResponse>());
    }
}
