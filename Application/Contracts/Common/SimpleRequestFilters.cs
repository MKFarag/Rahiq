namespace Application.Contracts.Common;

public record SimpleRequestFilters
{
    public int PageNumber { get; init; } = 1;
    public int PageSize { get; init; } = 10;
}

#region Validation

public class SimpleRequestFiltersValidator : AbstractValidator<SimpleRequestFilters>
{
    public SimpleRequestFiltersValidator()
    {
        RuleFor(x => x.PageNumber)
            .GreaterThan(0);

        RuleFor(x => x.PageSize)
            .InclusiveBetween(1, 250);
    }
}

#endregion
