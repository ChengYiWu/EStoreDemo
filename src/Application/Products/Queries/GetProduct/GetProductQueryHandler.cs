using Application.Common.Exceptions;
using Application.Common.Interfaces;
using Application.Common.Models;
using Application.Common.Services.FileService;
using Application.Products.Queries.Models;
using Dapper;
using Domain.Product;
using MediatR;
using static System.Net.Mime.MediaTypeNames;

namespace Application.Products.Queries.GetProduct;

public class GetProductQueryHandler : IRequestHandler<GetProductQuery, ProductResponse>
{
    private readonly ISqlConnectionFactory _sqlConnectionFactory;
    private readonly IProductFileUploadService _productFileUploadService;

    public GetProductQueryHandler(ISqlConnectionFactory sqlConnectionFactory, IProductFileUploadService productFileUploadService)
    {
        _sqlConnectionFactory = sqlConnectionFactory;
        _productFileUploadService = productFileUploadService;
    }

    public async Task<ProductResponse> Handle(GetProductQuery request, CancellationToken cancellationToken)
    {
        await using var conn = _sqlConnectionFactory.CreateConnection();

        var sql = $@"
            SELECT 
				[Product].[Id],
				[Product].[Name],
                [Product].[Description],
				[Product].[Brand],
				[Product].[Weight],
				[Product].[Dimensions],
				[CreatedUser].[UserName] AS [CreatedUserName],
				[ProductImage].[Id] AS [Id],
				[ProductImage].[OriFileName] AS [OriFileName],
				[ProductImage].[FileName] AS [FileName],
                [ProductImage].[Path] AS [Path],
				[ProductItem].[Id],
				[ProductItem].[Name],
				[ProductItem].[Price],
				[ProductItem].[StockQuantity],
				[ProductItem].[IsActive],
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
				) AS [CancelledOrderCount],
				[ProductItemImage].[Id] AS [Id],
				[ProductItemImage].[OriFileName] AS [OriFileName],
				[ProductItemImage].[FileName] AS [FileName],
                [ProductItemImage].[Path] AS [Path]
			FROM [Product]
			JOIN [User] AS [CreatedUser]
				ON [CreatedUser].[Id] = [Product].[CreatedBy]
			LEFT JOIN [ProductItem]
				ON [ProductItem].[ProductId] = [Product].[Id]
			LEFT JOIN [Attachment] AS [ProductImage]
				ON [ProductImage].[ProductId] = [Product].[Id]
			LEFT JOIN [Attachment] AS [ProductItemImage]
				ON [ProductItemImage].[ProductItemId] = [ProductItem].[Id]
			WHERE [Product].[Id] = @ProductId
        ";

        var param = new
        {
            ProductId = request.Id
        };

        var productDictionary = new Dictionary<int, ProductResponse>();
        var productImageSet = new HashSet<int>();
        var productItemSet = new HashSet<int>();

        var productResponse = (await conn.QueryAsync<ProductResponse, ExistFile, ProductItemDTO, ExistFile, ProductResponse>(
                sql,
                (product, productImage, productItem, productItemImage) =>
                {
                    if (!productDictionary.TryGetValue(product.Id, out var productResponse))
                    {
                        productResponse = product;
                        productDictionary.Add(product.Id, productResponse);
                    }

                    if(productImage is not null && !productImageSet.Contains(productImage.Id) )
                    {
                        productImageSet.Add(productImage.Id);
                        productImage.Uri = _productFileUploadService.FromRelativePathToAbsoluteUri(productImage.Path);
                        productResponse.Images.Add(productImage);
                    }

                    if(!productItemSet.Contains(productItem.Id))
                    {
                        productItemSet.Add(productItem.Id);
                        productResponse.ProductItems.Add(productItem);

                        if(productItemImage is not null)
                        {
                            productItemImage.Uri = _productFileUploadService.FromRelativePathToAbsoluteUri(productItemImage.Path);
                            productItem.Image = productItemImage;
                        }
                    }

                    return productResponse;
                },
                param
            )).Distinct().SingleOrDefault();

        if (productResponse is null)
        {
            throw new NotFoundException($"未找到商品編號 {request.Id} 的商品。請確保您輸入的編號正確，或者商品可能已被刪除。");
        }

        return productResponse;
    }
}
