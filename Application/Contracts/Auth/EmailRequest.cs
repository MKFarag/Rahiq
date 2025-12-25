namespace Application.Contracts.Auth;

public record EmailRequest(
    string Email
);

#region Validation

public class EmailRequestValidator : AbstractValidator<EmailRequest>
{
    public EmailRequestValidator()
    {
        RuleFor(x => x.Email)
            .NotEmpty()
            .EmailAddress();
    }
}

#endregion