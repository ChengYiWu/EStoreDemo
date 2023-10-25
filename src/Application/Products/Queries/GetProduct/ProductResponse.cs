namespace Application.Products.Queries.GetProduct;

public class ProductResponse
{
    public string Name { get; set; }

    public string Description { get; set; }

    public string? Brand { get; set; }

    public string? Weight { get; set; }

    public string? Dimensions { get; set; }

    public IList<string> Images { get; set; }

    public IList<ProductItemDTO> ProductItems { get; set; }
}

public class ProductItemDTO
{
    public string Name { get; set; }

    public decimal Price { get; set; }

    public int StockQuantity { get; set; }

    public bool IsActive { get; set; }

    public string? Image { get; set; }
}
