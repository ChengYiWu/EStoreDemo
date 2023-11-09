using Application.Common.Models;

namespace Application.Products.Queries.GetProductList;

public class ProductListResponse
{
    public IEnumerable<ProductListItemDTO> Items { get; set; } = new List<ProductListItemDTO>();
}

public class ProductListItemDTO
{
    public int Id { get; set; }

    public string Name { get; set; }

    public IList<ProductListProductItemDTO> ProductItems { get; set; } = new List<ProductListProductItemDTO>();
}

public class ProductListProductItemDTO
{
    public int Id { get; set; }

    public string Name { get; set; }

    public decimal Price { get; set; }

    public int StockQuantity { get; set; }

}

