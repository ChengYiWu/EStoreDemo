namespace Application.Coupons.Queries.GetCouponList;

public class CouponListReponse
{
    public IEnumerable<CouponListItemDTO> Items { get; set; } = new List<CouponListItemDTO>();
}

public class CouponListItemDTO
{
    public int Id { get; set; }

    public string Title { get; set; }

    public string Code { get; set; }

    public string Type { get; set; }

    public decimal? PriceAmountDiscount { get; set; }

    public decimal? PricePercentDiscount { get; set; }

    public IList<CouponListItemProductDTO> ApplicableProducts { get; set; } = new List<CouponListItemProductDTO>();
}

public class CouponListItemProductDTO
{
    public int Id { get; set; }

    public string Name { get; set; }
}
