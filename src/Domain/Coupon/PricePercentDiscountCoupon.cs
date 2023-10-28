namespace Domain.Coupon;

public class PricePercentDiscountCoupon : Coupon
{
    public decimal PricePercentDiscount { get; set; }

    public override decimal GetDiscountPrice(decimal price)
    {
        return decimal.Round(price * (1 - PricePercentDiscount), 0);
    }
}
