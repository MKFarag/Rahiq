namespace Application.Feathers.Category.AddCategory;

public record AddCategoryCommand(CategoryRequest Request) : IRequest<Result<CategoryResponse>>;