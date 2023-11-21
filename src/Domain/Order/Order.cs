using Domain.Common;

namespace Domain.Order;

public class Order : BaseEntity<int>
{
    public string OrderNo { get; set; } = string.Empty;

    public string? Note { get; set; }

    public OrderStatus Status { get; set; }

    public ShippingInfo ShippingInfo { get; set; } = default!;

    /// <summary>
    /// 訂單總價
    /// </summary>
    public decimal TotalPrice { get; set; }

    public DateTimeOffset PlacedAt { get; set; }

    public string? CancelledReason { get; set; }

    public string? CancelledBy { get; set; }

    public DateTimeOffset? CancelledAt { get; set; }

    public string CustomerId { get; set; } = string.Empty;

    public int? UsedCouponId { get; set; }

    /// <summary>
    /// 簡單 Demo，故只可使用一張優惠券，實務上應該要有多個折扣
    /// </summary>
    public Coupon.Coupon? UsedCoupon { get; set; }

    /// <summary>
    /// 折扣價格 snapshot
    /// </summary>
    public decimal? PriceDiscount { get; set; }

    /// <summary>
    /// 最終總價（包含優惠券折扣與運費）
    /// </summary>
    public decimal FinalTotalPrice { get; set; }

    /// <summary>
    /// 可否編輯
    /// </summary>
    public bool? IsEditable { get; set; }

    public ICollection<OrderItem> OrderItems { get; set; } = new List<OrderItem>();
}
