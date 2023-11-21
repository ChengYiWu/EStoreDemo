using Application.Common.Exceptions;
using Application.Common.Interfaces;
using Dapper;
using MediatR;

namespace Application.Coupons.Queries.GetCoupon;

public class GetCouponQueryHandler : IRequestHandler<GetCouponQuery, CouponResponse>
{
    private readonly ISqlConnectionFactory _sqlConnectionFactory;

    public GetCouponQueryHandler(ISqlConnectionFactory sqlConnectionFactory)
    {
        _sqlConnectionFactory = sqlConnectionFactory;
    }

    public async Task<CouponResponse> Handle(GetCouponQuery request, CancellationToken cancellationToken)
    {
        await using var conn = _sqlConnectionFactory.CreateConnection();

        var couponDictionary = new Dictionary<int, CouponResponse>();

        var sql = @"
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
                [Coupon].[IsEditable],
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
            LEFT JOIN [Product]
                ON [Product].[Id] = [CouponApplicableProduct].[ProductId]
            WHERE [Coupon].[Id] = @Id
        ";

        var param = new { request.Id };

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

                if(applicableProduct is not null)
                {
                    couponResponse.ApplicableProducts.Add(applicableProduct);
                }

                return couponResponse;
            },
            param
            )).Distinct().FirstOrDefault();

        if (coupon is null)
        {
            throw new NotFoundException($"找不到優惠券（{request.Id}）。");
        }

        return coupon;
    }
}
