using Application.Common.Interfaces;
using Dapper;
using MediatR;

namespace Application.Products.Queries.GetProductList;

public class GetProductListQueryHanlder : IRequestHandler<GetProductListQuery, ProductListResponse>
{
    private readonly ISqlConnectionFactory _sqlConnectionFactory;

    public GetProductListQueryHanlder(ISqlConnectionFactory sqlConnectionFactory)
    {
        _sqlConnectionFactory = sqlConnectionFactory;
    }

    public async Task<ProductListResponse> Handle(GetProductListQuery request, CancellationToken cancellationToken)
    {
        await using var conn = _sqlConnectionFactory.CreateConnection();

        var sql = @"
            SELECT 
	            [Product].[Id],
	            [Product].[Name],
	            [ProductItem].[Id],
	            [ProductItem].[Name],
	            [ProductItem].[Price],
	            [ProductItem].[StockQuantity]
            FROM [Product]
            LEFT JOIN [ProductItem] 
	            ON [Product].[Id] = [ProductItem].[ProductId]
            WHERE [ProductItem].[IsActive] = 1
            ORDER BY [Product].[Id] ASC
        ";

        var productListItemDictionary = new Dictionary<int, ProductListItemDTO>();

        var productResponse = (await conn.QueryAsync<ProductListItemDTO, ProductListProductItemDTO, ProductListItemDTO>(
                sql,
                (product, productItem) =>
                {
                    if (!productListItemDictionary.TryGetValue(product.Id, out var productResponse))
                    {
                        productResponse = product;
                        productListItemDictionary.Add(product.Id, productResponse);
                    }

                    productResponse.ProductItems.Add(productItem);

                    return productResponse;
                }
            )).Distinct().ToList();

        return new ProductListResponse
        {
            Items = productResponse
        };
    }
}
