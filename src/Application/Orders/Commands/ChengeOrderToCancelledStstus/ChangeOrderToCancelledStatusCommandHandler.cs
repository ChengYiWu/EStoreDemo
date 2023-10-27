using Application.Common.Exceptions;
using Application.Common.Identity;
using Domain.Order;
using MediatR;

namespace Application.Orders.Commands.ChengeOrderToCancelledStstus;

public class ChangeOrderToCancelledStatusCommandHandler : IRequestHandler<ChangeOrderToCancelledStatusCommand, bool>
{
    private readonly IOrderRepository _orderRepository;
    private readonly ICurrentUser _currentUser;

    public ChangeOrderToCancelledStatusCommandHandler(IOrderRepository orderRepository, ICurrentUser currentUser)
    {
        _orderRepository = orderRepository;
        _currentUser = currentUser;
    }

    public async Task<bool> Handle(ChangeOrderToCancelledStatusCommand request, CancellationToken cancellationToken)
    {
        var currentUserId = _currentUser.GetCurrentUserId();

        var order = await _orderRepository.GetOrderByOrderNo(request.OrderNo);

        if (order is null)
        {
            throw new NotFoundException($"找不到訂單（{request.OrderNo}）。");
        }

        if (order.Status == OrderStatus.Cancelled)
        {
            throw new FailureException($"訂單狀態不符，無法變更。");
        }

        order.Status = OrderStatus.Cancelled;
        order.CancelledReason = request.Reason;
        order.CancelledAt = DateTimeOffset.Now;
        order.CancelledBy = currentUserId;

        _orderRepository.Update(order);
        await _orderRepository.SaveChangesAsync(cancellationToken);

        // TODO，通知顧客訂單已出貨

        return true;
    }
}
