using Application.Common.Interfaces;
using Application.Common.Models;
using Application.Common.Services.FileService;
using Application.Common.Utils;
using Application.Products.Queries.Models;
using Application.Users.Queries.GetUsers;
using Dapper;
using Domain.Product;
using MediatR;
using System.Collections.Generic;
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
			WHERE 1 = 1
	        {whereSql}	
		";

        var productMasterSql = $@"
			SELECT COUNT([Product].[Id])
			FROM [Product]
			WHERE 1 = 1
	        {whereSql}	

			SELECT 
				[Product].[Id],
				[Product].[Name],
				[Product].[Description],
				[Product].[Brand],
				[Product].[Weight],
				[Product].[Dimensions],
                [Product].[IsEditable],
				[CreatedUser].[UserName] AS [CreatedUserName]
			FROM [Product]
			JOIN [User] AS [CreatedUser]
				ON [CreatedUser].[Id] = [Product].[CreatedBy]
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

        var queryReader = await conn.QueryMultipleAsync(productMasterSql, param);

        var totalCount = await queryReader.ReadSingleAsync<int>();
        var productResponses = await queryReader.ReadAsync<ProductResponse>();

        var productItemDetailSql = $@"
			SELECT 
				[Product].[Id] AS [ProductId],
				[ProductItem].[Id],
				[ProductItem].[Name],
				[ProductItem].[Price],
				[ProductItem].[StockQuantity],
				[ProductItem].[IsActive],
				(
					SELECT SUM([OrderItem].[Quantity]) 
					FROM [Order]
					JOIN [OrderItem] ON [OrderItem].[OrderId] = [Order].[Id]
					WHERE [Order].[Status] = 'Placed' AND
						[OrderItem].[ProductItemId] =  [ProductItem].[Id] 
				) AS [PlacedOrderCount],
				(
					SELECT SUM([OrderItem].[Quantity]) 
					FROM [Order]
					JOIN [OrderItem] ON [OrderItem].[OrderId] = [Order].[Id]
					WHERE [Order].[Status] = 'Shipped' AND
						[OrderItem].[ProductItemId] =  [ProductItem].[Id] 
				) AS [ShippedOrderCount],
				(
					SELECT SUM([OrderItem].[Quantity]) 
					FROM [Order]
					JOIN [OrderItem] ON [OrderItem].[OrderId] = [Order].[Id]
					WHERE [Order].[Status] = 'Cancelled' AND
						[OrderItem].[ProductItemId] = [ProductItem].[Id] 
				) AS [CancelledOrderCount]
			FROM [Product]
			LEFT JOIN [ProductItem]
				ON [ProductItem].[ProductId] = [Product].[Id]
			WHERE [Product].[Id] IN @Ids
		";

        var detailParam = new
        {
            Ids = productResponses.Select(x => x.Id)
        };

        var productItems = await conn.QueryAsync<ProductItemDTO>(productItemDetailSql, detailParam);

        var productItemLookup = productItems
            .ToLookup(
                productItem => productItem.ProductId,
                productItem => productItem
            );

        var imagesSql = $@"
			SELECT 
				[ProductImage].[ProductId],
				[ProductImage].[Id] AS [Id],
				[ProductImage].[OriFileName] AS [OriFileName],
				[ProductImage].[FileName] AS [FileName],
                [ProductImage].[Path] AS [Path]
			FROM [Attachment] AS [ProductImage]
            WHERE [ProductId] IN @ProductIds

			SELECT 
				[ProductItemImage].[ProductItemId],
				[ProductItemImage].[Id] AS [Id],
				[ProductItemImage].[OriFileName] AS [OriFileName],
				[ProductItemImage].[FileName] AS [FileName],
                [ProductItemImage].[Path] AS [Path]
			FROM [Attachment] AS [ProductItemImage]
            WHERE [ProductItemId] IN @ProductItemIds
		";

        var imagesParam = new
        {
            ProductIds = productResponses.Select(x => x.Id),
            ProductItemIds = productItems.Select(x => x.Id)
        };

        var imagesReader = await conn.QueryMultipleAsync(imagesSql, imagesParam);

        var productImages = await imagesReader.ReadAsync();
        var productItemImages = await imagesReader.ReadAsync();

        var productImageLookup = productImages
            .ToLookup(
                productImage => productImage.ProductId,
                productImage => new ExistFile
                {
                    Id = productImage.Id,
                    OriFileName = productImage.OriFileName,
                    FileName = productImage.FileName,
                    Path = productImage.Path,
                    Uri = _productFileUploadService.FromRelativePathToAbsoluteUri(productImage.Path)
                }
            );

        var productItemImageLookup = productItemImages
            .ToLookup(
                productItemImage => productItemImage.ProductItemId,
                productItemImage => new ExistFile
                {
                    Id = productItemImage.Id,
                    OriFileName = productItemImage.OriFileName,
                    FileName = productItemImage.FileName,
                    Path = productItemImage.Path,
                    Uri = _productFileUploadService.FromRelativePathToAbsoluteUri(productItemImage.Path)
                }
            );

        foreach (var productResponse in productResponses)
        {
            if (productImageLookup.Contains(productResponse.Id))
            {
                productResponse.Images = productImageLookup[productResponse.Id].ToList();
            }

            if (productItemLookup.Contains(productResponse.Id))
            {
                productResponse.ProductItems = productItemLookup[productResponse.Id].ToList();
            }

            foreach (var productItem in productResponse.ProductItems)
            {
                if (productItemImageLookup.Contains(productItem.Id))
                {
                    productItem.Image = productItemImageLookup[productItem.Id].FirstOrDefault();
                }
            }
        }

        return new PaginatedList<ProductResponse>(productResponses, totalCount, pageNumber, pageSize);
    }
}
