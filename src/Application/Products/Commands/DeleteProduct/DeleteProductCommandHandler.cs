using Application.Common.Exceptions;
using Application.Common.Services.FileService;
using Domain.Product;
using MediatR;

namespace Application.Products.Commands.DeleteProduct;

public class DeleteProductCommandHandler : IRequestHandler<DeleteProductCommand, bool>
{
    private readonly IProductRepository _productRepository;
    private readonly IProductFileUploadService _productFileUploadService;

    public DeleteProductCommandHandler(IProductRepository productRepository, IProductFileUploadService productFileUploadService)
    {
        _productRepository = productRepository;
        _productFileUploadService = productFileUploadService;
    }

    public async Task<bool> Handle(DeleteProductCommand request, CancellationToken cancellationToken)
    {
        var product = await _productRepository.GetByIdAsync(request.Id);

        if (product == null)
        {
            throw new NotFoundException($"未找到商品編號 {request.Id} 的商品。請確保您輸入的編號正確，或者商品可能已被刪除。");
        }

        // 蒐集要刪除的檔案
        var images = new List<string>()
            .Concat(
                product.Images
                    .Select(image => image.Path)
            )
            .Concat(
                product.ProductItems
                    .Where(item => item.Image is not null)
                    .Select(productItem => productItem.Image!.Path)
            );

        _productRepository.Delete(product);
        await _productRepository.SaveChangesAsync(cancellationToken);

        // 刪除實際檔案
        await _productFileUploadService.DeleteFiles(images);

        return true;
    }
}
