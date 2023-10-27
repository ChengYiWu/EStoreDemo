using FluentValidation;

namespace Application.Orders.Queries.GetOrder;

public class GetOrderQueryValidator : AbstractValidator<GetOrderQuery>
{
    public GetOrderQueryValidator()
    {
        RuleFor(x => x.OrderNo)
            .NotEmpty()
            .Must(x => Guid.TryParse(x, out _)).WithMessage("OrderNo must be a valid Guid");
    }
}
