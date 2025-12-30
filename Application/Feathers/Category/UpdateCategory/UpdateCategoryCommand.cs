namespace Application.Feathers.Category.UpdateCategory;

public record UpdateCategoryCommand(int Id, CategoryRequest Request) : IRequest<Result>;
