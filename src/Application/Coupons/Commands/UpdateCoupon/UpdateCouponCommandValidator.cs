using Application.Common.Extensions;
using Domain.Coupon;
using FluentValidation;

namespace Application.Coupons.Commands.UpdateCoupon;

public class UpdateCouponCommandValidator : AbstractValidator<UpdateCouponCommand>
{
    public UpdateCouponCommandValidator()
    {
        RuleFor(x => x.Id)
            .NotEmpty();

        RuleFor(x => x.Title)
           .NotEmpty()
           .MaximumLength(128);

        RuleFor(x => x.Code)
            .NotEmpty()
            .MaximumLength(64);

        RuleFor(x => x.Description)
            .NotEmpty()
            .MaximumLength(1024);

        RuleFor(x => x.Type)
            .NotEmpty()
            .IsEnumName(typeof(CouponType));

        RuleFor(x => x.StartedAt)
            .Must(x => DateTime.TryParse(x, out _))
            .When(x => !string.IsNullOrEmpty(x.StartedAt));

        RuleFor(x => x.ExpiredAt)
            .Must(x => DateTime.TryParse(x, out _))
            .When(x => !string.IsNullOrEmpty(x.ExpiredAt));

        RuleFor(x => x.IsActive)
            .NotNull()
            .NullOrBoolean();

        RuleFor(x => x.ApplicableProductIds)
            .ForEach(y =>
            {
                y.NotEmpty();
            });
    }
}
