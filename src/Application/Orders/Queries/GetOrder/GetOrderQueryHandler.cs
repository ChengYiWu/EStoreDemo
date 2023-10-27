using Application.Common.Exceptions;
using Application.Common.Interfaces;
using Application.Orders.Queries.Models;
using Dapper;
using MediatR;

namespace Application.Orders.Queries.GetOrder;

public class GetOrderQueryHandler : IRequestHandler<GetOrderQuery, OrderResponse>
{
    private readonly ISqlConnectionFactory _sqlConnectionFactory;

    public GetOrderQueryHandler(ISqlConnectionFactory sqlConnectionFactory)
    {
        _sqlConnectionFactory = sqlConnectionFactory;
    }

    public async Task<OrderResponse> Handle(GetOrderQuery request, CancellationToken cancellationToken)
    {
        await using var conn = _sqlConnectionFactory.CreateConnection();

        var orderDictionary = new Dictionary<string, OrderResponse>();

        var sql = @"
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
			WHERE [Order].[OrderNo] = @OrderNo
			
        ";

        var param = new { request.OrderNo };

        var order = (await conn.QueryAsync<OrderResponse, OrderItemDTO, OrderResponse>(
            sql,
            (order, orderItem) =>
            {
                OrderResponse orderResponse;

                if (!orderDictionary.TryGetValue(order.OrderNo, out orderResponse))
                {
                    orderResponse = order;
                    orderDictionary.Add(order.OrderNo, orderResponse);
                }

                orderResponse.OrderItems.Add(orderItem);
                return orderResponse;
            },
            param,
            splitOn: "OrderItemId"
            )).Distinct().FirstOrDefault();

        if (order is null)
        {
            throw new NotFoundException($"找不到訂單（{request.OrderNo}）。");
        }

        return order;
    }
}
