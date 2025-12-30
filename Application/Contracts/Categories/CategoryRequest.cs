namespace Application.Contracts.Categories;

public record CategoryRequest(
    string Name
);

#region Validation

public class CategoryRequestValidator : AbstractValidator<CategoryRequest>
{
    public CategoryRequestValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty()
            .MaximumLength(50);
    }
}

#endregion