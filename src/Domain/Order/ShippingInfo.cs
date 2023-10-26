namespace Domain.Order;

public class ShippingInfo
{
    public string ContactPhone { get; set; } = string.Empty;

    public string ShippingAddress { get; set; } = string.Empty;

    public decimal ShippingFee { get; set; }

    public string? ShippedBy { get; set; }

    public DateTimeOffset? ShippedAt { get; set; }
}
