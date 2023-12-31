﻿namespace Application.Coupons.Queries.GetCoupons;

public class CouponResponse
{
    public int Id { get; set; }
    public string Title { get; set; }
    public string Code { get; set; }
    public string Description { get; set; }
    public DateTimeOffset StartedAt { get; set; }
    public DateTimeOffset ExpiredAt { get; set; }
    public string Type { get; set; }
    public bool IsActive { get; set; }
    public string CreatedUserId { get; set; }
    public string CreatedUserName { get; set; }
    public decimal? PriceAmountDiscount { get; set; }
    public decimal? PricePercentDiscount { get; set; }
    public bool? IsEditable { get; set; }
    public int UsedOrderCount { get; set; }
    public IList<CouponApplicableProductDTO> ApplicableProducts { get; set; } = new List<CouponApplicableProductDTO>();
}

public class CouponApplicableProductDTO
{
    public int CouponId { get; set; }
    public int Id { get; set; }
    public string Name { get; set; }
    public int ProductItemCount { get; set; }
}
