using Application.Common.Models;
using Application.Orders.Queries.Models;
using Domain.Order;
using MediatR;

namespace Application.Orders.Queries.GetOrders;

public record GetOrdersQuery(
    string? Search,
    string? Status,
    string? StartAt,
    string? EndAt,
    int PageNumber = 1,
    int PageSize = 10
) : IRequest<PaginatedList<OrderResponse>>;
