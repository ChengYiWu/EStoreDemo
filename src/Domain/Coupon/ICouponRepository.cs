using Domain.Common;

namespace Domain.Coupon;

public interface ICouponRepository : IRepository<Coupon, int>
{
    public Task<bool> IsCodeDuplicated(string code, int? couponId = null);

    public Task<bool> IsAnyOrderUsed(int couponId);
}
