namespace Domain.Coupon;

public class CouponApplicableProduct
{
    public int CouponId { get; set; }

    public Coupon Coupon { get; set; } = default!;

    public int ProductId { get; set; }

    public Product.Product Product { get; set; } = default!;
}
