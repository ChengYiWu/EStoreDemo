using Application.Common.Exceptions;
using Domain.Coupon;
using Domain.Product;
using MediatR;

namespace Application.Coupons.Commands.UpdateCoupon;

public class UpdateCouponCommandHandler : IRequestHandler<UpdateCouponCommand, bool>
{
    private readonly ICouponRepository _couponRepository;
    private readonly IProductRepository _productRepository;

    public UpdateCouponCommandHandler(ICouponRepository couponRepository, IProductRepository productRepository)
    {
        _couponRepository = couponRepository;
        _productRepository = productRepository;
    }

    public async Task<bool> Handle(UpdateCouponCommand request, CancellationToken cancellationToken)
    {
        if (request.ApplicableProductIds.Any() && !await _productRepository.IsAllProductsExist(request.ApplicableProductIds))
        {
            throw new FailureException("部分商品資料不存在，請確認後重新新增。");
        }

        var coupon = await _couponRepository.GetByIdAsync(request.Id);

        if (coupon is null)
        {
            throw new NotFoundException($"優惠券不存在（{request.Id}），請重新確認。");
        }

        var isAnyOrderUsed = await _couponRepository.IsAnyOrderUsed(request.Id);

        // 未使用過的優惠券才能修改優惠券數值與領取代碼
        if (!isAnyOrderUsed)
        {
            if (await _couponRepository.IsCodeDuplicated(request.Code, request.Id))
            {
                throw new FailureException($"領取代碼已重複（{request.Code}），請確認後重新新增。");
            }

            coupon.Code = request.Code;

            if (Enum.TryParse(request.Type, out CouponType couponType))
            {
                if (coupon is PriceAmountDiscountCoupon priceAmountCoupon &&
                    couponType == CouponType.PriceAmountDiscount &&
                    request.PriceAmountDiscount is not null)
                {
                    priceAmountCoupon.PriceAmountDiscount = request.PriceAmountDiscount.Value;
                }
                else if (coupon is PricePercentDiscountCoupon pricePercentCoupon &&
                    couponType == CouponType.PricePercentDiscount &&
                    request.PricePercentDiscount is not null)
                {
                    pricePercentCoupon.PricePercentDiscount = request.PricePercentDiscount.Value;
                }
                else
                {
                    throw new FailureException("資料不正確，請確認後重新新增。");
                }
            }
            else
            {
                throw new FailureException("資料不正確，請確認後重新新增。");
            }

        }

        coupon.Title = request.Title;
        coupon.StartedAt = request.StartedAt is not null ? DateTimeOffset.Parse(request.StartedAt) : null;
        coupon.ExpiredAt = request.ExpiredAt is not null ? DateTimeOffset.Parse(request.ExpiredAt) : null;
        coupon.Description = request.Description;
        coupon.IsActive = request.IsActive ?? false;

        // 移除可用商品
        coupon.ApplicableProducts = coupon.ApplicableProducts.Where(x => request.ApplicableProductIds.Contains(x.ProductId)).ToList();

        // 加入可用商品
        foreach (var productId in request.ApplicableProductIds)
        {
            if (coupon.ApplicableProducts.Any(x => x.ProductId == productId))
            {
                continue;
            }

            coupon.ApplicableProducts.Add(new CouponApplicableProduct
            {
                ProductId = productId
            });
        }

        _couponRepository.Update(coupon);
        await _couponRepository.SaveChangesAsync(cancellationToken);

        return true;
    }
}
