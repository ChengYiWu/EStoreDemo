using Application.Common.Exceptions;
using Application.Common.Identity;
using Domain.Coupon;
using Domain.Order;
using Domain.Product;
using MediatR;

namespace Application.Orders.Comnmands.PlaceOrder;

public class PlaceOrderCommandHandler : IRequestHandler<PlaceOrderCommand, string>
{
    private readonly IOrderRepository _orderRepository;
    private readonly IProductRepository _productRepository;
    private readonly ICouponRepository _couponRepository;
    private readonly IIdentityService _identityService;
    private readonly ICurrentUser _currentUser;

    public PlaceOrderCommandHandler(
        IOrderRepository orderRepository,
        IProductRepository productRepository,
        ICouponRepository couponRepository,
        IIdentityService identityService,
        ICurrentUser currentUser)
    {
        _orderRepository = orderRepository;
        _productRepository = productRepository;
        _couponRepository = couponRepository;
        _identityService = identityService;
        _currentUser = currentUser;
    }

    public async Task<string> Handle(PlaceOrderCommand request, CancellationToken cancellationToken)
    {
        var currentUserId = _currentUser.GetCurrentUserId();

        var customer = await _identityService.GetUserByIdAsync(request.CustomerId);

        if (customer is null)
        {
            throw new NotFoundException("找不到顧客資料。");
        }

        Coupon? coupon = null;
        if (request.CouponCode is not null)
        {
            var couponFromDb = await _couponRepository.GetByCodeAsync(request.CouponCode);

            if (couponFromDb is null)
            {
                throw new NotFoundException($"找不到優惠券資料（{request.CouponCode}）。");
            }

            coupon = couponFromDb;
        }

        var requestProductItemIds = request.Items.Select(item => item.ProductItemId).ToList();
        var requestProductItemMap = request.Items
            .ToDictionary(x => x.ProductItemId, x => x);

        var products = await _productRepository.GetProductByProductItemIds(requestProductItemIds);

        var placedProductItems = products
                .SelectMany(product => product.ProductItems.Where(item => requestProductItemIds.Contains(item.Id))
                .ToList());

        // 商品項目檢查
        foreach (var item in placedProductItems)
        {
            // 是否開賣檢查
            if (!item.IsActive)
            {
                throw new FailureException($"您所選取的商品「{item.Name}」已下架，故無法進行結帳，請重新確認。");
            }

            // 是否庫存足夠檢查（TODO: 若要嚴謹避免超賣的話，還需檢查 rowVersion 或是用 Select Update 的方式去更新庫存，Update 的條件要扣除此單後的庫存 >= 0 才執行）
            var requestProductItem = requestProductItemMap[item.Id];
            if (item.StockQuantity - requestProductItem.Quantity < 0)
            {
                throw new FailureException($"您所選取的商品「{item.Name}」庫存不足，故無法進行結帳，請重新確認。");
            }

            item.StockQuantity -= requestProductItem.Quantity;
        }

        // 更新庫存（TODO: 嚴謹的情況可以在 Update 時檢查 rowVersion，若修改時 rowVersion 有變更，則表示資料舊的，需重新計算）
        foreach (var product in products)
        {
            _productRepository.Update(product);
        }

        await _productRepository.SaveChangesAsync(cancellationToken);

        // 建立訂單
        var newOrder = new Order
        {
            OrderNo = Guid.NewGuid().ToString(),
            Note = request.Note,
            Status = OrderStatus.Placed,
            ShippingInfo = new ShippingInfo
            {
                ContactPhone = request.ContactPhone,
                ShippingAddress = request.ShippingAddress,
                ShippingFee = 100, // 簡單 Demo，故寫死運費
            },
            CustomerId = customer.Id,
            PlacedAt = DateTimeOffset.Now,
            TotalPrice = placedProductItems.Sum(item => item.Price * requestProductItemMap[item.Id].Quantity),
            OrderItems = placedProductItems.Select(item =>
            {
                return new OrderItem
                {
                    ProductItemId = item.Id,
                    Quantity = requestProductItemMap[item.Id].Quantity,
                    Price = item.Price
                };
            }).ToList()
        };

        // 計算折扣
        if (coupon is not null)
        {
            newOrder.PriceDiscount = coupon.GetDiscountPrice(newOrder.TotalPrice);
            newOrder.UsedCoupon = coupon;
        }

        // 計算最終總價
        newOrder.FinalTotalPrice = decimal.Round(newOrder.TotalPrice - (newOrder.PriceDiscount ?? 0m) + newOrder.ShippingInfo.ShippingFee, 0);

        _orderRepository.Add(newOrder);
        await _orderRepository.SaveChangesAsync(cancellationToken);

        return newOrder.OrderNo;
    }
}
