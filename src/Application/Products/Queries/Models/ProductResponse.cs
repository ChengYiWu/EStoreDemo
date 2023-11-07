using Application.Common.Models;

namespace Application.Products.Queries.Models;

public class ProductResponse
{
    public int Id { get; set; }

    public string Name { get; set; }

    public string Description { get; set; }

    public string? Brand { get; set; }

    public string? Weight { get; set; }

    public string? Dimensions { get; set; }

    public string CreatedUserName { get; set; }

    public IList<ExistFile> Images { get; set; } = new List<ExistFile>();

    public IList<ProductItemDTO> ProductItems { get; set; } = new List<ProductItemDTO>();
}

public class ProductItemDTO
{
    public int ProductId { get; set; }

    public int Id { get; set; }

    public string Name { get; set; }

    public decimal Price { get; set; }

    public int StockQuantity { get; set; }

    public bool IsActive { get; set; }

    public ExistFile? Image { get; set; }

    //public int ImageId { get; set; }

    //public string? ImagePath { get; set; }

    public int PlacedOrderCount { get; set; }

    public int ShippedOrderCount { get; set; }

    public int CancelledOrderCount { get; set; }
}
