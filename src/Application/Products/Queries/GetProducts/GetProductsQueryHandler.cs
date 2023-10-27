using Application.Common.Interfaces;
using Application.Common.Models;
using Application.Common.Services.FileService;
using Application.Common.Utils;
using Application.Products.Queries.Models;
using Dapper;
using MediatR;
using System.Text;

namespace Application.Products.Queries.GetProducts;

public class GetProductsQueryHandler : IRequestHandler<GetProductsQuery, PaginatedList<ProductResponse>>
{
    private readonly ISqlConnectionFactory _sqlConnectionFactory;
    private readonly IProductFileUploadService _productFileUploadService;

    public GetProductsQueryHandler(ISqlConnectionFactory sqlConnectionFactory, IProductFileUploadService productFileUploadService)
    {
        _sqlConnectionFactory = sqlConnectionFactory;
        _productFileUploadService = productFileUploadService;
    }

    public async Task<PaginatedList<ProductResponse>> Handle(GetProductsQuery request, CancellationToken cancellationToken)
    {
        await using var conn = _sqlConnectionFactory.CreateConnection();

        var whereSql = new StringBuilder();

        if (request.Search is not null && !string.IsNullOrWhiteSpace(request.Search))
        {
            whereSql.Append(@" AND 
                ( 
                    [Product].[Name] LIKE @Search 
                )
            ");
        }

        var countSql = $@"
	        SELECT COUNT([Product].[Id])
			FROM [Product]
			JOIN [User] AS [CreatedUser]
				ON [CreatedUser].[Id] = [Product].[CreatedBy]
			WHERE 1 = 1
	        {whereSql}	
		";

        var sql = $@"
            SELECT 
				[Product].[Id],
				[Product].[Name],
				[Product].[Brand],
				[Product].[Weight],
				[Product].[Dimensions],
				[CreatedUser].[UserName] AS [CreatedUserName],
				[ProductImage].[Path] AS [ImagePath],
				[ProductItem].[Id],
				[ProductItem].[Name],
				[ProductItem].[Price],
				[ProductItem].[StockQuantity],
				[ProductItem].[IsActive],
				[ProductItemImage].[Id] AS [ImageId],
				[ProductItemImage].[Path] AS [ImagePath],
				(
					SELECT COUNT([OrderItem].[Id]) 
					FROM [Order]
					JOIN [OrderItem] ON [OrderItem].[OrderId] = [Order].[Id]
					WHERE [Order].[Status] = 'Placed' AND
						[OrderItem].[ProductItemId] =  [ProductItem].[Id] 
				) AS [PlacedOrderCount],
				(
					SELECT COUNT([OrderItem].[Id]) 
					FROM [Order]
					JOIN [OrderItem] ON [OrderItem].[OrderId] = [Order].[Id]
					WHERE [Order].[Status] = 'Shipped' AND
						[OrderItem].[ProductItemId] =  [ProductItem].[Id] 
				) AS [ShippedOrderCount],
				(
					SELECT COUNT([OrderItem].[Id]) 
					FROM [Order]
					JOIN [OrderItem] ON [OrderItem].[OrderId] = [Order].[Id]
					WHERE [Order].[Status] = 'Cancelled' AND
						[OrderItem].[ProductItemId] =  [ProductItem].[Id] 
				) AS [CancelledOrderCount]
			FROM [Product]
			JOIN [User] AS [CreatedUser]
				ON [CreatedUser].[Id] = [Product].[CreatedBy]
			LEFT JOIN [ProductItem]
				ON [ProductItem].[ProductId] = [Product].[Id]
			LEFT JOIN [Attachment] AS [ProductImage]
				ON [ProductImage].[ProductId] = [Product].[Id]
			LEFT JOIN [Attachment] AS [ProductItemImage]
				ON [ProductItemImage].[ProductItemId] = [ProductItem].[Id]
			WHERE 1 = 1
            {whereSql}
			ORDER BY [Product].[Id] DESC
			OFFSET @Offset ROWS
			FETCH NEXT @Next ROWS ONLY
        ";

        (int Offset, int Next, int pageSize, int pageNumber) = QueryHelper.GetPagingParams(request.PageSize, request.PageNumber);
        var param = new
        {
            Search = $"%{request.Search}%",
            Offset,
            Next
        };

        var totalCount = await conn.QuerySingleAsync<int>(countSql, param);

        var productDictionary = new Dictionary<int, ProductResponse>();

        var productResponses = (await conn.QueryAsync<ProductResponse, string, ProductItemDTO, ProductResponse>(
                sql,
                (product, imagePath, productItem) =>
                {
                    if (!productDictionary.TryGetValue(product.Id, out var productResponse))
                    {
                        productResponse = product;
                        productDictionary.Add(product.Id, productResponse);
                    }

                    productResponse.Images.Add(_productFileUploadService.FromRelativePathToAbsoluteUri(imagePath));

                    if (productItem.ImagePath is not null)
                    {
                        productItem.ImagePath = _productFileUploadService.FromRelativePathToAbsoluteUri(productItem.ImagePath);
                    }
                    productResponse.ProductItems.Add(productItem);

                    return productResponse;
                },
                param,
                splitOn: "ImagePath,Id"
            )).Distinct().ToList();

        return new PaginatedList<ProductResponse>(productResponses, totalCount, pageNumber, pageSize);
    }
}
