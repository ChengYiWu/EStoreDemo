using MediatR;

namespace Application.Products.Queries.GetProductList;

public record GetProductListQuery(
) : IRequest<ProductListResponse>;
