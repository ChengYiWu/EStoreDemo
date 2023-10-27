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
                [Id],
                [Title],
                [Code],
                [Type],
                [PriceAmountDiscount],
                [PricePercentDiscount]
            FROM [Coupon]
        ";

        var items = (await conn.QueryAsync<CouponListItemDTO>(sql, cancellationToken)).ToList();

        return new CouponListReponse
        {
            Items = items
        };
    }
}
