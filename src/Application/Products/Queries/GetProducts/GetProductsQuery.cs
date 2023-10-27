using Application.Common.Models;
using Application.Products.Queries.Models;
using MediatR;

namespace Application.Products.Queries.GetProducts;

public record GetProductsQuery(
    string? Search,
    int PageNumber = 1,
    int PageSize = 10
) : IRequest<PaginatedList<ProductResponse>>;
