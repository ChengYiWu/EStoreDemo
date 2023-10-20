using MediatR;

namespace Application.Products.Commands.CreateProduct;

public sealed record CreateProductCommand(
    string Name,
    string Content
) : IRequest<int>;