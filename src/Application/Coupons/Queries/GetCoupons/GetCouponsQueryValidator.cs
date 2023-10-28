using Application.Common.Extensions;
using Domain.Coupon;
using FluentValidation;

namespace Application.Coupons.Queries.GetCoupons;

public class GetCouponsQueryValidator : AbstractValidator<GetCouponsQuery>
{
    public GetCouponsQueryValidator()
    {
        RuleFor(x => x.IsActive)
            .NullOrBoolean();

        RuleFor(x => x.Type)
            .IsEnumName(typeof(CouponType));
    }
}
