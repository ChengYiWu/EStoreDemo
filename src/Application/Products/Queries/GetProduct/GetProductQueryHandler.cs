﻿using Application.Common.Exceptions;
using Application.Common.Interfaces;
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
			WHERE [Product].[Id] = @ProductId
        ";

        var param = new
        {
            ProductId = request.Id
        };

        var productDictionary = new Dictionary<int, ProductResponse>();

        var productResponse = (await conn.QueryAsync<ProductResponse, string, ProductItemDTO, ProductResponse>(
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
            )).Distinct().SingleOrDefault();

        if (productResponse is null)
        {
            throw new NotFoundException($"未找到商品編號 {request.Id} 的商品。請確保您輸入的編號正確，或者商品可能已被刪除。");
        }

        return productResponse;
    }
}
