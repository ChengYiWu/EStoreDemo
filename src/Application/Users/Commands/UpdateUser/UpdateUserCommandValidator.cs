using FluentValidation;

namespace Application.Users.Commands.UpdateUser;

public class UpdateUserCommandValidator : AbstractValidator<UpdateUserCommand>
{
    public UpdateUserCommandValidator()
    {
        RuleFor(x => x.UserName)
            .NotEmpty()
            .NotNull()
            .MaximumLength(256);

        RuleFor(x => x.Email)
            .NotEmpty()
            .NotNull()
            .MaximumLength(256)
            .EmailAddress();
    }
}
