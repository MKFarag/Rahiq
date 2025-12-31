namespace Application.Contracts.Types;

public record TypeRequest(
    string Name
);

#region Validation

public class TypeRequestValidator : AbstractValidator<TypeRequest>
{
    public TypeRequestValidator()
    {
        RuleFor(x => x.Name)
            .MaximumLength(50);
    }
}

#endregion