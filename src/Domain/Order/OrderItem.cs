using Domain.Common;

namespace Domain.Order;

public class OrderItem : BaseEntity<int>
{
    /// <summary>
    /// Unit Price
    /// </summary>
    public decimal Price { get; set; }

    public int Quantity { get; set; }

    public int ProductItemId { get; set; }
}
