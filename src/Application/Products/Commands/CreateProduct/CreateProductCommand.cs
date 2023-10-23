using MediatR;

namespace Application.Products.Commands.CreateProduct;

public sealed record CreateProductCommand(
    string Name,
    string Description,
    string ImageOriFileName,
    string ImageTmpFileName
) : IRequest<int>;