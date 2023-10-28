namespace Domain.Coupon;

public class PriceAmountDiscountCoupon : Coupon
{
    public decimal PriceAmountDiscount { get; set; }

    public override decimal GetDiscountPrice(decimal price)
    {
        return PriceAmountDiscount;
    }
}
