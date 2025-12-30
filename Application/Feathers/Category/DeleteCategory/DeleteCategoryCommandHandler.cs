namespace Application.Feathers.Category.DeleteCategory;

public class DeleteCategoryCommandHandler(IUnitOfWork unitOfWork) : IRequestHandler<DeleteCategoryCommand, Result>
{
    private readonly IUnitOfWork _unitOfWork = unitOfWork;

    public async Task<Result> Handle(DeleteCategoryCommand request, CancellationToken cancellationToken = default)
    {
        if (await _unitOfWork.Categories.GetAsync([request.Id], cancellationToken) is not { } category)
            return Result.Failure(CategoryErrors.NotFound);

        if (await _unitOfWork.Products.AnyAsync(x => x.CategoryId == request.Id, cancellationToken))
            return Result.Failure(CategoryErrors.InUse);

        _unitOfWork.Categories.Delete(category);
        await _unitOfWork.CompleteAsync(cancellationToken);

        return Result.Success();
    }
}
