namespace Domain.Product;

public class ProductItemImageAttachment : Attachment.Attachment
{
    public int ProductItemId { get; set; }

    public ProductItem? ProductItem { get; set; }
}
