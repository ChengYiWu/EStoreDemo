namespace Domain.Coupon;

public class PricePercentDiscountCoupon : Coupon
{
    public decimal PricePercentDiscount { get; set; }

    public override decimal GetDiscountPrice(decimal price)
    {
        return decimal.Round(price * ((100 - PricePercentDiscount) / 100), 0);
    }
}
