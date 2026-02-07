namespace Application.Feathers.Category.AddCategory;

public class AddCategoryCommandHandler(IUnitOfWork unitOfWork, ICacheService cache) : IRequestHandler<AddCategoryCommand, Result<CategoryResponse>>
{
    private readonly IUnitOfWork _unitOfWork = unitOfWork;
    private readonly ICacheService _cache = cache;

    public async Task<Result<CategoryResponse>> Handle(AddCategoryCommand command, CancellationToken cancellationToken = default)
    {
        if (await _unitOfWork.Categories.AnyAsync(x => command.Request.Name == x.Name, cancellationToken))
            return Result.Failure<CategoryResponse>(CategoryErrors.DuplicatedName);

        var category = new Domain.Entities.Category
        {
            Name = command.Request.Name
        };

        await _unitOfWork.Categories.AddAsync(category, cancellationToken);
        await _unitOfWork.CompleteAsync(cancellationToken);

        await _cache.RemoveAsync(Cache.Keys.Categories, cancellationToken);

        return Result.Success(category.Adapt<CategoryResponse>());
    }
}
