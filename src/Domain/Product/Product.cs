using Domain.Common;

namespace Domain.Product;

public class Product : BaseEntity<int>
{
    public string Name { get; set; }

    public string Description { get; set; }

    public ICollection<ProductImageAttachment> Images { get; set; } = new List<ProductImageAttachment>();

    public DateTimeOffset CreatedAt { get; set; }

    public string? CreatedBy { get; set; }
}
