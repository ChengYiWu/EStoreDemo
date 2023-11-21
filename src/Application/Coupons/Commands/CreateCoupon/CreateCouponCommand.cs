using Domain.Coupon;
using MediatR;

namespace Application.Coupons.Commands.CreateCoupon;

public class CreateCouponCommand : IRequest<int>
{
    public string Title { get; set; }
    public string Code { get; set; }
    public string Description { get; set; }
    public string Type { get; set; }
    public decimal? PriceAmountDiscount { get; set; }
    public decimal? PricePercentDiscount { get; set; }
    public string? StartedAt { get; set; }
    public string? ExpiredAt { get; set; }
    public bool? IsActive { get; set; }
    public List<int> ApplicableProductIds { get; set; } = new List<int>();
}