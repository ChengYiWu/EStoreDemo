using Application.Common.Exceptions;
using Application.Common.Services.FileService;
using Domain.Product;
using MediatR;

namespace Application.Products.Queries.GetProduct;

public class GetProductQueryHandler : IRequestHandler<GetProductQuery, ProductResponse>
{
    private readonly IProductRepository _productRepository;
    private readonly IProductFileUploadService _productFileUploadService;

    public GetProductQueryHandler(IProductRepository productRepository, IProductFileUploadService productFileUploadService)
    {
        _productRepository = productRepository;
        _productFileUploadService = productFileUploadService;
    }

    public async Task<ProductResponse> Handle(GetProductQuery request, CancellationToken cancellationToken)
    {
        var product = await _productRepository.GetByIdAsync(request.Id) ?? throw new NotFoundException($"未找到商品編號 {request.Id} 的商品。請確保您輸入的編號正確，或者商品可能已被刪除。");

        return new ProductResponse()
        {
            Name = product.Name,
            Description = product.Description,
            Brand = product.Brand,
            Weight = product.Weight,
            Dimensions = product.Dimensions,
            Images = product.Images
                .Select(image => _productFileUploadService.FromRelativePathToAbsoluteUri(image.Path))
                .ToList(),
            ProductItems = product.ProductItems.Select(productItem => new ProductItemDTO()
            {
                Name = productItem.Name,
                Price = productItem.Price,
                StockQuantity = productItem.StockQuantity,
                IsActive = productItem.IsActive,
                Image = productItem.Image is not null 
                    ? _productFileUploadService.FromRelativePathToAbsoluteUri(productItem.Image.Path) 
                    : null
            }).ToList()
        };
    }
}
