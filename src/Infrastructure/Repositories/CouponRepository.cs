using Domain.Coupon;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositories;

public class CouponRepository : BaseRepository<Coupon, int>, ICouponRepository
{
    public CouponRepository(EStoreContext context) : base(context)
    {
    }

    public async override Task<Coupon?> GetByIdAsync(int id)
    {
        return await _dbSet
            .Include(c => c.ApplicableProducts)
            .ThenInclude(applicableProduct => applicableProduct.Product)
            .FirstOrDefaultAsync(c => c.Id == id);
    }

    public async Task<bool> IsCodeDuplicated(string code, int? couponId = null)
    {
        if (couponId.HasValue)
        {
            return await _query.AnyAsync(c => c.Code == code && c.Id != couponId.Value);
        }

        return await _query.AnyAsync(c => c.Code == code);
    }

    public async Task<bool> IsAnyOrderUsed(int couponId)
    {
        return await _context.Orders.AnyAsync(order => order.UsedCouponId == couponId);
    }

    public async Task<Coupon?> GetByCodeAsync(string code)
    {
        return await _query.FirstOrDefaultAsync(c => c.Code == code);
    }
}
