using FluentValidation;

namespace Application.Orders.Commands.ChengeOrderToCancelledStstus;

public class ChangeOrderToCancelledStatusCommandValidator : AbstractValidator<ChangeOrderToCancelledStatusCommand>
{
    public ChangeOrderToCancelledStatusCommandValidator()
    {
        RuleFor(x => x.OrderNo)
            .NotEmpty()
            .Must(x => Guid.TryParse(x, out _)).WithMessage("OrderNo must be a valid Guid");

        RuleFor(x => x.Reason)
            .NotEmpty();
    }
}
