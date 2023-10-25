using Application.Common.Models.Commands;
using MediatR;

namespace Application.Products.Commands.CreateProduct;

public record CreateProductCommand(
    string Name,
    string Description,
    string? Brand,
    string? Weight,
    string? Dimensions,
    List<UploadedFileDTO> Images,
    List<CreateProductItemDTO> ProductItems
) : IRequest<int>;

public record CreateProductItemDTO(
    string Name,
    decimal Price,
    int StockQuantity,
    bool? IsActive,
    UploadedFileDTO? Image
);

