using FluentValidation;

namespace Application.Orders.Comnmands.PlaceOrder;

public class PlaceOrderCommandValidator : AbstractValidator<PlaceOrderCommand>
{
    public PlaceOrderCommandValidator()
    {
        RuleFor(x => x.ContactPhone)
            .NotNull()
            .NotEmpty();

        RuleFor(x => x.ShippingAddress)
            .NotNull()
            .NotEmpty();

        RuleFor(x => x.CustomerId)
            .NotNull()
            .NotEmpty();

        RuleFor(x => x.CouponCode)
            .NotEmpty();

        RuleFor(x => x.Items)
            .NotEmpty()
            .NotNull()
            .ForEach(x =>
            {
                x.NotNull();
                x.ChildRules(y =>
                {
                    y.RuleFor(z => z.Quantity)
                        .NotNull()
                        .NotEmpty()
                        .GreaterThanOrEqualTo(1);

                    y.RuleFor(z => z.ProductItemId)
                        .NotNull()
                        .NotEmpty();
                });
            });
    }
}
