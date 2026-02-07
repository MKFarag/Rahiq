namespace Application.Feathers.Category.UpdateCategory;

public class UpdateCategoryCommandHandler(IUnitOfWork unitOfWork, ICacheService cache) : IRequestHandler<UpdateCategoryCommand, Result>
{
    private readonly IUnitOfWork _unitOfWork = unitOfWork;
    private readonly ICacheService _cache = cache;

    public async Task<Result> Handle(UpdateCategoryCommand command, CancellationToken cancellationToken = default)
    {
        if (await _unitOfWork.Categories.GetAsync([command.Id], cancellationToken) is not { } category)
            return Result.Failure(CategoryErrors.NotFound);

        if (await _unitOfWork.Categories.AnyAsync(c => c.Name == command.Request.Name && c.Id != command.Id, cancellationToken))
            return Result.Failure(CategoryErrors.DuplicatedName);

        category = command.Request.Adapt(category);

        await _unitOfWork.CompleteAsync(cancellationToken);

        await _cache.RemoveByTagAsync(Cache.Tags.Category, cancellationToken);

        return Result.Success();
    }
}
