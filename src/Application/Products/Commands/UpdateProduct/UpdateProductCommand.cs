using Application.Common.Models.Commands;
using MediatR;

namespace Application.Products.Commands.UpdateProduct;

public class UpdateProductCommand : IRequest<bool>
{
    public int Id { get; set; }
    public string Name { get; set; }
    public string Description { get; set; }
    public string? Brand { get; set; }
    public string? Weight { get; set; }
    public string? Dimensions { get; set; }
    public IList<string> OriImageIds { get; set; }
    public IList<UploadedFileDTO> NewImages { get; set; }
    public IList<UpdateProductItemDTO> ProductItems { get; set; }
}

public record UpdateProductItemDTO(
    int? Id,
    string Name,
    decimal Price,
    int StockQuantity,
    bool? IsActive,
    string? OriImageId,
    UploadedFileDTO? NewImage
);