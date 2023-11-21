using Application.Common.Exceptions;
using Application.Common.Identity;
using Domain.Coupon;
using Domain.Order;
using MediatR;

namespace Application.Orders.Commands.ChangeOrderToShippedStatus;

public class ChangeOrderToShippedStatusCommandHandler : IRequestHandler<ChangeOrderToShippedStatusCommand, bool>
{
    private readonly IOrderRepository _orderRepository;
    private readonly ICurrentUser _currentUser;

    public ChangeOrderToShippedStatusCommandHandler(IOrderRepository orderRepository, ICurrentUser currentUser)
    {
        _orderRepository = orderRepository;
        _currentUser = currentUser;
    }

    public async Task<bool> Handle(ChangeOrderToShippedStatusCommand request, CancellationToken cancellationToken)
    {
        var currentUserId = _currentUser.GetCurrentUserId();

        var order = await _orderRepository.GetOrderByOrderNo(request.OrderNo);

        if (order is null)
        {
            throw new NotFoundException($"找不到訂單（{request.OrderNo}）。");
        }

        if (order.IsEditable.HasValue && !order.IsEditable.Value)
        {
            throw new FailureException("不可變更資料。");
        }

        if (order.Status != OrderStatus.Placed || order.Status == OrderStatus.Shipped)
        {
            throw new FailureException($"訂單狀態不符，無法變更。");
        }

        order.Status = OrderStatus.Shipped;
        order.ShippingInfo.ShippedAt = DateTimeOffset.Now;
        order.ShippingInfo.ShippedBy = currentUserId;

        _orderRepository.Update(order);
        await _orderRepository.SaveChangesAsync(cancellationToken);

        // TODO，通知顧客訂單已出貨

        return true;
    }
}
