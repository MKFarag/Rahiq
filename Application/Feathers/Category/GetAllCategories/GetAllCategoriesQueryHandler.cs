namespace Application.Feathers.Category.GetAllCategories;

public class GetAllCategoriesQueryHandler(IUnitOfWork unitOfWork) : IRequestHandler<GetAllCategoriesQuery, IEnumerable<CategoryResponse>>
{
    private readonly IUnitOfWork _unitOfWork = unitOfWork;

    public async Task<IEnumerable<CategoryResponse>> Handle(GetAllCategoriesQuery request, CancellationToken cancellationToken = default)
        => await _unitOfWork.Categories.GetAllProjectionAsync<CategoryResponse>(cancellationToken);
}
