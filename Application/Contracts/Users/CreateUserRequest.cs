namespace Application.Contracts.Users;

public record CreateUserRequest(
    string FirstName,
    string LastName,
    string Email,
    string Password,
    IList<string> Roles
);


#region Validation

public class CreateUserRequestValidator : AbstractValidator<CreateUserRequest>
{
    public CreateUserRequestValidator()
    {
        RuleFor(x => x.FirstName)
            .NotEmpty()
            .Length(2, 50);

        RuleFor(x => x.LastName)
            .NotEmpty()
            .Length(2, 50);

        RuleFor(x => x.Email)
            .NotEmpty()
            .EmailAddress();

        RuleFor(x => x.Password)
            .NotEmpty()
            .Matches(RegexPatterns.Password)
            .WithMessage("Password should be at least 8 characters and should contains digit, Lowercase, Uppercase and NonAlphanumeric");

        RuleFor(x => x.Roles)
            .NotNull()
            .NotEmpty();

        RuleFor(x => x.Roles)
            .Must(x => x.Distinct().Count() == x.Count)
            .WithMessage("You cannot add duplicated role for the same user")
            .When(x => x.Roles != null);
    }
}

#endregion
