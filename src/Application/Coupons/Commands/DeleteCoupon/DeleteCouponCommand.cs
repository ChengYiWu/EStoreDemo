using MediatR;

namespace Application.Coupons.Commands.DeleteCoupon;

public record DeleteCouponCommand(
    int Id
) : IRequest<bool>;
