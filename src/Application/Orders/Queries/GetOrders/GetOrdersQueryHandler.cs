using Application.Common.Interfaces;
using Application.Common.Models;
using Application.Common.Utils;
using Application.Orders.Queries.Models;
using Dapper;
using MediatR;
using Microsoft.Data.SqlClient;
using System.Text;

namespace Application.Orders.Queries.GetOrders;

public class GetOrdersQueryHandler : IRequestHandler<GetOrdersQuery, PaginatedList<OrderResponse>>
{
    private readonly ISqlConnectionFactory _sqlConnectionFactory;

    public GetOrdersQueryHandler(ISqlConnectionFactory sqlConnectionFactory)
    {
        _sqlConnectionFactory = sqlConnectionFactory;
    }

    public async Task<PaginatedList<OrderResponse>> Handle(GetOrdersQuery request, CancellationToken cancellationToken)
    {
        await using SqlConnection conn = _sqlConnectionFactory.CreateConnection();

        var whereSql = new StringBuilder();

        if (request.Search is not null && !string.IsNullOrWhiteSpace(request.Search))
        {
            whereSql.Append(@" AND 
                ( 
                    [ShippedUser].[UserName] LIKE @Search 
                    OR [ShippedUser].[Email] LIKE @Search
                    OR [Order].[OrderNo] LIKE @Search 
                )
            ");
        }

        if (request.Status is not null)
        {
            whereSql.Append(@"AND
                (
                    [Order].[Status] = @Status  
                )
            ");
        }

        if (request.StartAt is not null && !string.IsNullOrWhiteSpace(request.StartAt))
        {
            whereSql.Append(@"AND
                (
                    [Order].[PlacedAt] >= @StartAt
                )
            ");
        }

        if (request.EndAt is not null && !string.IsNullOrWhiteSpace(request.EndAt))
        {
            whereSql.Append(@"AND
                (
                    [Order].[PlacedAt] <= @EndAt
                )
            ");
        }

        var countSql = @$"
            SELECT COUNT(*)
            FROM [Order]
			JOIN [User] AS [Customer]
				ON [Customer].[Id] = [Order].[CustomerId]
			LEFT JOIN [User] AS [ShippedUser]
				ON [ShippedUser].[Id] = [Order].[ShippedBy]
			LEFT JOIN [User] AS [CancelledUser]
				ON [CancelledUser].[Id] = [Order].[CancelledBy]
            WHERE 1 = 1
            {whereSql}
        ";

        var sql = @$"
            SELECT 
				[Order].[OrderNo],
				[Order].[Note],
				[Order].[Status],
				[Order].[ContactPhone],
				[Order].[ShippingAddress],
				[Order].[ShippingFee],
				[ShippedUser].[UserName] AS [ShippedUserName],
				[Order].[ShippedAt],
				[Order].[TotalPrice],
                [Order].[PriceDiscount],    
                [Order].[FinalTotalPrice],
				[Order].[PlacedAt],
				[CancelledUser].[UserName] AS [CancelledUserName],
				[Order].[CancelledAt],
				[Order].[CancelledReason],
				[OrderItem].[Id] AS [OrderItemId],
				[OrderItem].[Price] AS [OrderItemPrice],
				[OrderItem].[Quantity] AS [OrderItemQuantity],
				[Product].[Id] AS [ProductId],
				[Product].[Name] AS [ProductName],
				[ProductItem].[Name] AS [ProductItemName]
			FROM [Order]
			JOIN [OrderItem]
				 ON [Order].[Id] = [OrderItem].[OrderId]
			JOIN [ProductItem] 
				ON [ProductItem].[Id] = [OrderItem].[ProductItemId]
			JOIN [Product]
				ON [Product].[Id] = [ProductItem].[ProductId]
			JOIN [User] AS [Customer]
				ON [Customer].[Id] = [Order].[CustomerId]
			LEFT JOIN [User] AS [ShippedUser]
				ON [ShippedUser].[Id] = [Order].[ShippedBy]
			LEFT JOIN [User] AS [CancelledUser]
				ON [CancelledUser].[Id] = [Order].[CancelledBy]
			WHERE 1 = 1
            {whereSql}
            ORDER BY [Order].[PlacedAt] DESC
            OFFSET @Offset ROWS
            FETCH NEXT @Next ROWS ONLY
        ";

        (int Offset, int Next, int pageSize, int pageNumber) = QueryHelper.GetPagingParams(request.PageSize, request.PageNumber);
        var param = new
        {
            Search = $"%{request.Search}%",
            request.Status,
            request.StartAt,
            request.EndAt,
            Offset,
            Next
        };

        var totalCount = await conn.QuerySingleAsync<int>(countSql, param);

        var orderDictionary = new Dictionary<string, OrderResponse>();

        var orderResponses = (await conn.QueryAsync<OrderResponse, OrderItemDTO, OrderResponse>(
             sql,
             (order, orderItem) =>
             {
                 if (!orderDictionary.TryGetValue(order.OrderNo, out var orderResponse))
                 {
                     orderResponse = order;
                     orderDictionary.Add(order.OrderNo, orderResponse);
                 }

                 orderResponse.OrderItems.Add(orderItem);
                 return orderResponse;
             },
             param,
             splitOn: "OrderItemId"
             )).Distinct().ToList();

        return new PaginatedList<OrderResponse>(orderResponses, totalCount, pageNumber, pageSize);
    }
}
