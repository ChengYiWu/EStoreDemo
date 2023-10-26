using Domain.Order;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositories;

public class OrderRepository : BaseRepository<Order, int>, IOrderRepository
{
    public OrderRepository(EStoreContext context) : base(context)
    {
    }

    public async Task<Order?> GetOrderByOrderNo(string orderNo)
    {
        return await _query
            .Where(x => x.OrderNo == orderNo)
            .Include(x => x.OrderItems)
            .FirstOrDefaultAsync();
    }
}
