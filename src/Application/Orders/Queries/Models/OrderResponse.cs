using Domain.Order;

namespace Application.Orders.Queries.Models;

public class OrderResponse
{
    public string OrderNo { get; set; }
    public string Note { get; set; }
    public string Status { get; set; }
    public string ContactPhone { get; set; }
    public string ShippingAddress { get; set; }
    public decimal ShippingFee { get; set; }
    public string ShippedUserName { get; set; }
    public DateTimeOffset ShippedAt { get; set; }
    public decimal TotalPrice { get; set; }
    public decimal? PriceDiscount { get; set; }
    public decimal FinalTotalPrice { get; set; }
    public DateTimeOffset PlacedAt { get; set; }
    public string CancelledUserName { get; set; }
    public DateTimeOffset CancelledAt { get; set; }
    public string CancelledReason { get; set; }
    public List<OrderItemDTO> OrderItems { get; set; } = new List<OrderItemDTO>();
}

public class OrderItemDTO
{
    public int OrderItemId { get; set; }
    public decimal OrderItemPrice { get; set; }
    public int OrderItemQuantity { get; set; }
    public int ProductId { get; set; }
    public string ProductName { get; set; }
    public string ProductItemName { get; set; }

}