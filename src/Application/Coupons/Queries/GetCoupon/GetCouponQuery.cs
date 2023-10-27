using MediatR;

namespace Application.Coupons.Queries.GetCoupon;

public record GetCouponQuery(
    int Id
) : IRequest<CouponResponse>;
