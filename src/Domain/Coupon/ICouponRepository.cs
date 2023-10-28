using Domain.Common;

namespace Domain.Coupon;

public interface ICouponRepository : IRepository<Coupon, int>
{
    public Task<bool> IsCodeDuplicated(string code, int? couponId = null);

    public Task<Coupon?> GetByCodeAsync(string code);

    public Task<bool> IsAnyOrderUsed(int couponId);
}
