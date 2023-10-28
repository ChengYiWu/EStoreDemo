using Domain.Common;
using System.Dynamic;

namespace Domain.Coupon;

public abstract class Coupon : BaseEntity<int>
{
    public string Title { get; set; } = string.Empty;

    public string Code { get; set; } = string.Empty;

    public string Description { get; set; } = string.Empty;

    public DateTimeOffset? StartedAt { get; set; }

    public DateTimeOffset? ExpiredAt { get; set; }

    public CouponType Type { get; set; } = CouponType.PriceAmountDiscount;

    public bool IsActive { get; set; }

    public string CreatedBy { get; set; } = string.Empty;

    public DateTimeOffset CreatedAt { get; set; }

    public string? UpadtedBy { get; set; }

    public DateTimeOffset? UpdatedAt { get; set; }

    public ICollection<CouponApplicableProduct> ApplicableProducts { get; set; } = new List<CouponApplicableProduct>();

    public abstract decimal GetDiscountPrice(decimal price);
}
