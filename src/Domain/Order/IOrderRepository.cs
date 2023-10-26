using Domain.Common;

namespace Domain.Order;

public interface IOrderRepository : IRepository<Order, int>
{
    public Task<Order?> GetOrderByOrderNo(string orderNo);
}
