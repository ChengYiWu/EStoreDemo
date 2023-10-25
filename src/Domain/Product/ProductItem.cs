using Domain.Common;

namespace Domain.Product;

public class ProductItem : BaseEntity<int>
{
    /// <summary>
    /// 商品編號
    /// </summary>
    public int ProductId { get; set; }

    /// <summary>
    /// 商品項目名稱
    /// </summary>
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// 商品項目價格
    /// </summary>
    public decimal Price { get; set; }

    /// <summary>
    /// 商品項目庫存數量
    /// </summary>
    public int StockQuantity { get; set; }

    /// <summary>
    /// 是否啟用
    /// </summary>
    public bool IsActive { get; set; }

    /// <summary>
    /// 商品項目圖片
    /// </summary>
    public ProductItemImageAttachment? Image { get; set; }

    public DateTimeOffset CreatedAt { get; set; }

    public string? CreatedBy { get; set; }
}
