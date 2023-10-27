using Domain.Order;
using FluentValidation;

namespace Application.Orders.Queries.GetOrders;

public class GetOrdersQueryValidator : AbstractValidator<GetOrdersQuery>
{
    public GetOrdersQueryValidator()
    {
        RuleFor(x => x.Status)
            .IsEnumName(typeof(OrderStatus));

        RuleFor(x => x.EndAt)
            .Must(x => DateTime.TryParse(x, out _))
            .When(x => !string.IsNullOrEmpty(x.EndAt));

        RuleFor(x => x.StartAt)
            .Must(x => DateTime.TryParse(x, out _))
            .When(x => !string.IsNullOrEmpty(x.StartAt));

        RuleFor(x => x.PageNumber)
            .GreaterThanOrEqualTo(1);

        RuleFor(x => x.PageSize)
            .GreaterThanOrEqualTo(1)
            .LessThanOrEqualTo(1000);
    }
}
