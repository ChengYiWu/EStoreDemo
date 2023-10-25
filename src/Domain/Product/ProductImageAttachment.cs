namespace Domain.Product;

public class ProductImageAttachment : Attachment.Attachment
{
    public int ProductId { get; set; }

    public Product? Product { get; set; }
}
