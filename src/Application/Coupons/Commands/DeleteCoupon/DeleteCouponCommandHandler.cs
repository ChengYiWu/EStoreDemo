using Application.Common.Exceptions;
using Domain.Coupon;
using MediatR;

namespace Application.Coupons.Commands.DeleteCoupon;

public class DeleteCouponCommandHandler : IRequestHandler<DeleteCouponCommand, bool>
{
    private readonly ICouponRepository _couponRepository;

    public DeleteCouponCommandHandler(ICouponRepository couponRepository)
    {
        _couponRepository = couponRepository;
    }

    public async Task<bool> Handle(DeleteCouponCommand request, CancellationToken cancellationToken)
    {
        var coupon = await _couponRepository.GetByIdAsync(request.Id);

        if (coupon == null)
        {
            throw new NotFoundException($"優惠券不存在（{request.Id}），請重新確認。");
        }

        if (coupon.IsEditable.HasValue && !coupon.IsEditable.Value)
        {
            throw new FailureException("不可刪除資料。");
        }

        var isAnyOrderUsed = await _couponRepository.IsAnyOrderUsed(request.Id);

        if (isAnyOrderUsed)
        {
            throw new FailureException("已有訂單使用此優惠券，無法刪除。");
        }

        _couponRepository.Delete(coupon);
        await _couponRepository.SaveChangesAsync(cancellationToken);

        return true;
    }
}
