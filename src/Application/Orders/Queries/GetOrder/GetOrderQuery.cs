using Application.Orders.Queries.Models;
using MediatR;

namespace Application.Orders.Queries.GetOrder;

public record GetOrderQuery(
    string OrderNo
) : IRequest<OrderResponse>;
