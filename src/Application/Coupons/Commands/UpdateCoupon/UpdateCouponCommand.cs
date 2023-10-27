using Domain.Coupon;
using MediatR;

namespace Application.Coupons.Commands.UpdateCoupon;

public class UpdateCouponCommand : IRequest<bool>
{
    public int Id { get; set; }
    public string Title { get; set; }
    public string Code { get; set; }
    public string Description { get; set; }
    public string Type { get; set; }
    public decimal? PriceAmountDiscount { get; set; }
    public decimal? PricePercentDiscount { get; set; }
    public string? StartedAt { get; set; }
    public string? ExpiredAt { get; set; }
    public bool? IsActive { get; set; }
    public IList<int> ApplicableProductIds { get; set; }
}
