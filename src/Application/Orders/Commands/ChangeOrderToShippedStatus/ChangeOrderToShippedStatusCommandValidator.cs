using FluentValidation;

namespace Application.Orders.Commands.ChangeOrderToShippedStatus;

public class ChangeOrderToShippedStatusCommandValidator : AbstractValidator<ChangeOrderToShippedStatusCommand>
{
    public ChangeOrderToShippedStatusCommandValidator()
    {
        RuleFor(x => x.OrderNo)
            .NotEmpty()
            .Must(x => Guid.TryParse(x, out _)).WithMessage("OrderNo must be a valid Guid");
    }
}
