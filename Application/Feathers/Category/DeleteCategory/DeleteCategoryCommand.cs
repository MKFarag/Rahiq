namespace Application.Feathers.Category.DeleteCategory;

public record DeleteCategoryCommand(int Id) : IRequest<Result>;
