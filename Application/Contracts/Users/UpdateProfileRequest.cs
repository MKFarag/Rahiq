namespace Application.Contracts.Users;

public record UpdateProfileRequest(
    string FirstName,
    string LastName
);

#region Validation

public class UpdateProfileRequestValidator : AbstractValidator<UpdateProfileRequest>
{
    public UpdateProfileRequestValidator()
    {
        RuleFor(x => x.FirstName)
            .NotEmpty()
            .Length(2, 50);

        RuleFor(x => x.LastName)
            .NotEmpty()
            .Length(2, 50);
    }
}

#endregion