using Domain.Common;

namespace Domain.Product;

public class Product : BaseEntity<int>
{
    /// <summary>
    /// 商品名稱
    /// </summary>
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// 商品描述
    /// </summary>
    public string Description { get; set; } = string.Empty;

    /// <summary>
    /// 品牌
    /// </summary>
    public string? Brand { get; set; }

    /// <summary>
    /// 重量
    /// </summary>
    public string? Weight { get; set; }

    /// <summary>
    /// 尺寸
    /// </summary>
    public string? Dimensions { get; set; }

    /// <summary>
    /// 商品項目
    /// </summary>
    public ICollection<ProductItem> ProductItems { get; set; } = new List<ProductItem>();

    /// <summary>
    /// 商品圖片
    /// </summary>
    public ICollection<ProductImageAttachment> Images { get; set; } = new List<ProductImageAttachment>();

    /// <summary>
    /// 建立時間
    /// </summary>
    public DateTimeOffset CreatedAt { get; set; }

    /// <summary>
    /// 建立者
    /// </summary>
    public string? CreatedBy { get; set; }

    /// <summary>
    /// 修改時間
    /// </summary>
    public DateTimeOffset? UpdatedAt { get; set; }

    /// <summary>
    /// 修改者
    /// </summary>
    public string? UpdatedBy { get; set; }
}
