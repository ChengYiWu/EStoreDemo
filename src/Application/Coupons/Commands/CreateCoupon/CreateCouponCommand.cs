using Domain.Coupon;
using MediatR;

namespace Application.Coupons.Commands.CreateCoupon;

public record CreateCouponCommand(
    string Title,
    string Code,
    string Description,
    string Type,
    decimal? PriceAmountDiscount,
    decimal? PricePercentDiscount,
    string? StartedAt,
    string? ExpiredAt,
    bool? IsActive,
    IList<int> ApplicableProductIds
) : IRequest<int>;
