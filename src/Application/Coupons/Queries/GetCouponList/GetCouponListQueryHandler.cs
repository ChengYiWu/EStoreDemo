using Application.Common.Interfaces;
using Dapper;
using MediatR;

namespace Application.Coupons.Queries.GetCouponList;

public class GetCouponListQueryHandler : IRequestHandler<GetCouponListQuery, CouponListReponse>
{
    private readonly ISqlConnectionFactory _sqlConnectionFactory;

    public GetCouponListQueryHandler(ISqlConnectionFactory sqlConnectionFactory)
    {
        _sqlConnectionFactory = sqlConnectionFactory;
    }

    public async Task<CouponListReponse> Handle(GetCouponListQuery request, CancellationToken cancellationToken)
    {
        await using var conn = _sqlConnectionFactory.CreateConnection();

        var sql = @"
            SELECT
                [Coupon].[Id],
                [Coupon].[Title],
                [Coupon].[Code],
                [Coupon].[Type],
                [Coupon].[PriceAmountDiscount],
                [Coupon].[PricePercentDiscount],
                [Product].[Id],
                [Product].[Name]
            FROM [Coupon]
            LEFT JOIN [CouponApplicableProduct] 
                ON [CouponApplicableProduct].[CouponId] = [Coupon].[Id]
            LEFT JOIN [Product]
                ON [Product].[Id] = [CouponApplicableProduct].[ProductId]
            WHERE [Coupon].[IsActive] = 1
        ";

        var couponDictionary = new Dictionary<int, CouponListItemDTO>();

        var items = (await conn.QueryAsync<CouponListItemDTO, CouponListItemProductDTO, CouponListItemDTO>(sql, (coupon, applicableProduct) =>
        {
            CouponListItemDTO couponResponse;

            if (!couponDictionary.TryGetValue(coupon.Id, out couponResponse))
            {
                couponResponse = coupon;
                couponDictionary.Add(coupon.Id, couponResponse);
            }

            if (applicableProduct is not null)
            {
                couponResponse.ApplicableProducts.Add(applicableProduct);
            }

            return couponResponse;

        }, cancellationToken)).Distinct().ToList();

        return new CouponListReponse
        {
            Items = items
        };
    }
}
