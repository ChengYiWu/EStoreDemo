using MediatR;

namespace Application.Coupons.Queries.GetCouponList;

public record GetCouponListQuery(
) : IRequest<CouponListReponse>;
