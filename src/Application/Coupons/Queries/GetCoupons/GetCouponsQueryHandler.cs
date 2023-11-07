using Application.Common.Interfaces;
using Application.Common.Models;
using Application.Common.Utils;
using Dapper;
using MediatR;
using System.Text;

namespace Application.Coupons.Queries.GetCoupons;

public class GetCouponsQueryHandler : IRequestHandler<GetCouponsQuery, PaginatedList<CouponResponse>>
{
    private readonly ISqlConnectionFactory _sqlConnectionFactory;

    public GetCouponsQueryHandler(ISqlConnectionFactory sqlConnectionFactory)
    {
        _sqlConnectionFactory = sqlConnectionFactory;
    }

    public async Task<PaginatedList<CouponResponse>> Handle(GetCouponsQuery request, CancellationToken cancellationToken)
    {
        await using var conn = _sqlConnectionFactory.CreateConnection();

        var couponDictionary = new Dictionary<int, CouponResponse>();

        var whereSql = new StringBuilder();

        if (request.Search is not null && !string.IsNullOrWhiteSpace(request.Search))
        {
            whereSql.Append(@" AND 
                ( 
                    [Coupon].[Title] LIKE @Search 
                    OR [Coupon].[Code] LIKE @Search
                )
            ");
        }

        if (request.IsActive is not null)
        {
            whereSql.Append(@"AND
                (
                    [Coupon].[IsActive] = @IsActive  
                )
            ");
        }

        if (request.Type is not null)
        {
            whereSql.Append(@"AND
                (
                    [Coupon].[Type] = @Type  
                )
            ");
        }

        var couponMasterSql = @$"
            SELECT 
                COUNT([Coupon].[Id])
            FROM [Coupon]
            WHERE 1 = 1
            {whereSql}

            SELECT 
                [Coupon].[Id],
                [Coupon].[Title],
                [Coupon].[Code],
                [Coupon].[Description],
                [Coupon].[StartedAt],
                [Coupon].[ExpiredAt],
                [Coupon].[Type],
                [Coupon].[IsActive],
                [Coupon].[CreatedBy] AS [CreatedUserId],
                [CreatedUser].[UserName] AS [CreatedUserName],
                [Coupon].[PriceAmountDiscount],
                [Coupon].[PricePercentDiscount]
            FROM [Coupon]
            LEFT JOIN [User] AS [CreatedUser]
                ON [CreatedUser].[Id] = [Coupon].[CreatedBy]
            WHERE 1 = 1
            {whereSql}
            ORDER BY [Coupon].[Id] DESC
			OFFSET @Offset ROWS
			FETCH NEXT @Next ROWS ONLY
        ";

        (int Offset, int Next, int pageSize, int pageNumber) = QueryHelper.GetPagingParams(request.PageSize, request.PageNumber);
        var param = new
        {
            Search = $"%{request.Search}%",
            request.IsActive,
            request.Type,
            Offset,
            Next
        };

        var queryReader = await conn.QueryMultipleAsync(couponMasterSql, param);

        var totalCount = await queryReader.ReadSingleAsync<int>();
        var couponResponses = await queryReader.ReadAsync<CouponResponse>();

        var productDetailSql = @$"
            SELECT 
                [Coupon].[Id] AS [CouponId],
                [Product].[Id],
                [Product].[Name],
                (
                    SELECT COUNT([ProductItem].[Id])
                    FROM [ProductItem] WHERE [ProductItem].[ProductId] = [Product].[Id]
                ) AS [ProductItemCount]
            FROM [Coupon]
            LEFT JOIN [User] AS [CreatedUser]
                ON [CreatedUser].[Id] = [Coupon].[CreatedBy]
            LEFT JOIN [CouponApplicableProduct] 
                ON [CouponApplicableProduct].[CouponId] = [Coupon].[Id]
            JOIN [Product]
                ON [Product].[Id] = [CouponApplicableProduct].[ProductId]
            WHERE [Coupon].[Id] IN @Ids
        ";

        var detailParam = new
        {
            Ids = couponResponses.Select(x => x.Id)
        };

        var products = await conn.QueryAsync<CouponApplicableProductDTO>(productDetailSql, detailParam);

        var productLookup = products
           .ToLookup(
               product => product.Id,
               product => product
           );
        
        foreach (var coupon in couponResponses)
        {
            if(productLookup.Contains(coupon.Id))
            {
                coupon.ApplicableProducts = productLookup[coupon.Id].ToList();
            }
        }

        return new PaginatedList<CouponResponse>(couponResponses, totalCount, pageNumber, pageSize);
    }
}
