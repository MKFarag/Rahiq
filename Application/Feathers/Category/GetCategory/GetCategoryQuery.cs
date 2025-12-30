namespace Application.Feathers.Category.GetCategory;

public record GetCategoryQuery(int Id) : IRequest<Result<CategoryResponse>>;
