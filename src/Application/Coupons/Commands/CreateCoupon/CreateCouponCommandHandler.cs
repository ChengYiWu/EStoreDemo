using Application.Common.Exceptions;
using Application.Common.Identity;
using Domain.Coupon;
using Domain.Product;
using MediatR;

namespace Application.Coupons.Commands.CreateCoupon;

public class CreateCouponCommandHandler : IRequestHandler<CreateCouponCommand, int>
{
    private readonly ICouponRepository _couponRepository;
    private readonly IProductRepository _productRepository;
    private readonly ICurrentUser _currentUser;

    public CreateCouponCommandHandler(ICouponRepository couponRepository, IProductRepository productRepository, ICurrentUser currentUser)
    {
        _couponRepository = couponRepository;
        _productRepository = productRepository;
        _currentUser = currentUser;
    }

    public async Task<int> Handle(CreateCouponCommand request, CancellationToken cancellationToken)
    {
        var currentUserId = _currentUser.GetCurrentUserId();

        if (request.ApplicableProductIds.Any() && !await _productRepository.IsAllProductsExist(request.ApplicableProductIds))
        {
            throw new FailureException("部分商品資料不存在，請確認後重新新增。");
        }

        if (await _couponRepository.IsCodeDuplicated(request.Code))
        {
            throw new FailureException($"領取代碼已重複（{request.Code}），請確認後重新新增。");
        }

        Coupon coupon;

        if (Enum.TryParse(request.Type, out CouponType couponType))
        {
            if (couponType == CouponType.PriceAmountDiscount && request.PriceAmountDiscount is not null)
            {
                coupon = new PriceAmountDiscountCoupon
                {
                    PriceAmountDiscount = request.PriceAmountDiscount.Value
                };
            }
            else if (couponType == CouponType.PricePercentDiscount && request.PricePercentDiscount is not null)
            {
                coupon = new PricePercentDiscountCoupon
                {
                    PricePercentDiscount = request.PricePercentDiscount.Value
                };
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

        coupon.Title = request.Title;
        coupon.Code = request.Code;
        coupon.StartedAt = request.StartedAt is not null ? DateTimeOffset.Parse(request.StartedAt) : null;
        coupon.ExpiredAt = request.ExpiredAt is not null ? DateTimeOffset.Parse(request.ExpiredAt) : null;
        coupon.Description = request.Description;
        coupon.IsActive = request.IsActive ?? false;
        coupon.CreatedBy = currentUserId;
        coupon.CreatedAt = DateTimeOffset.Now;

        foreach (var productId in request.ApplicableProductIds)
        {
            coupon.ApplicableProducts.Add(new CouponApplicableProduct
            {
                ProductId = productId
            });
        }

        _couponRepository.Add(coupon);
        await _couponRepository.SaveChangesAsync(cancellationToken);

        return coupon.Id;
    }
}