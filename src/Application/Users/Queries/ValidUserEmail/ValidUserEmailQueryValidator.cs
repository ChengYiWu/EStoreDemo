using FluentValidation;

namespace Application.Users.Queries.ValidUserEmail;

public class ValidUserEmailQueryValidator : AbstractValidator<ValidUserEmailQuery>
{
    public ValidUserEmailQueryValidator()
    {
        RuleFor(x => x.Email)
            .NotNull()
            .NotEmpty()
            .EmailAddress();
    }
}
