using FluentValidation;

namespace Application.Users.Commands.ChangeUserPassword;

public class ChangeUserPasswordCommandValidator : AbstractValidator<ChangeUserPasswordCommand>
{
    public ChangeUserPasswordCommandValidator()
    {
        RuleFor(x => x.NewPassword)
            .NotEmpty()
            .NotNull()
            .MinimumLength(6);
    }
}
