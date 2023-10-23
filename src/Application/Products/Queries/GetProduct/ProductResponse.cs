namespace Application.Products.Queries.GetProduct;

public class ProductResponse
{
    public string Name { get; set; }

    public string Description { get; set; }

    public IList<string> Images { get; set; }
}
