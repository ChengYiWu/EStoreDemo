using FluentValidation;

namespace Application.Users.Commands.CreateUser;

public class CreateUserCommandValidator : AbstractValidator<CreateUserCommand>
{
    public CreateUserCommandValidator()
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

        RuleFor(x => x.Password)
            .NotEmpty()
            .NotNull()
            .MinimumLength(6);
    }
}
