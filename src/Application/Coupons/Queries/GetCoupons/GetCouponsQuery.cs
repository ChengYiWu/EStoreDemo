using Application.Common.Models;
using MediatR;

namespace Application.Coupons.Queries.GetCoupons;

public record GetCouponsQuery(
    string? Search,
    bool? IsActive,
    string? Type,
    int PageNumber = 1,
    int PageSize = 10
) : IRequest<PaginatedList<CouponResponse>>;
