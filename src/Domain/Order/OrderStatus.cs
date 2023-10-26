namespace Domain.Order;

/// <summary>
/// 簡單 Demo，故不考慮付款、退貨等情境
/// </summary>
public enum OrderStatus
{
    Placed,
    Shipped,
    Cancelled
}
