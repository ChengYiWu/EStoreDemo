using Domain.Common;

namespace Domain.Order;

public class Order : BaseEntity<int>
{
    public string OrderNo { get; set; } = string.Empty;

    public string? Note { get; set; }

    public OrderStatus Status { get; set; }

    public ShippingInfo ShippingInfo { get; set; } = default!;

    public decimal TotalPrice { get; set; }

    public DateTimeOffset PlacedAt { get; set; }

    public string? CancelledReason { get; set; }

    public string? CancelledBy { get; set; }

    public DateTimeOffset? CancelledAt { get; set; }

    public string CustomerId { get; set; } = string.Empty;

    public ICollection<OrderItem> OrderItems { get; set; } = new List<OrderItem>();
}
