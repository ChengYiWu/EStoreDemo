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

        var countSql = @$"
            SELECT 
                COUNT([Coupon].[Id])
            FROM [Coupon]
            WHERE 1 = 1
            {whereSql}
        ";

        var sql = @$"
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
                [Coupon].[PricePercentDiscount],
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
            WHERE 1 = 1
            {whereSql}
        ";

        (int Offset, int Next, int pageSize, int pageNumber) = QueryHelper.GetPagingParams(request.PageSize, request.PageNumber);
        var param = new
        {
            request.Search,
            request.IsActive,
            request.Type
        };

        var totalCount = await conn.QuerySingleAsync<int>(countSql, param);

        var coupon = (await conn.QueryAsync<CouponResponse, CouponApplicableProductDTO, CouponResponse>(
            sql,
            (coupon, applicableProduct) =>
            {
                CouponResponse couponResponse;

                if (!couponDictionary.TryGetValue(coupon.Id, out couponResponse))
                {
                    couponResponse = coupon;
                    couponDictionary.Add(coupon.Id, couponResponse);
                }

                couponResponse.ApplicableProducts.Add(applicableProduct);

                return couponResponse;
            },
            param
            )).Distinct().ToList();

        return new PaginatedList<CouponResponse>(coupon, totalCount, pageNumber, pageSize);
    }
}
