using Application.Common.Exceptions;
using Application.Common.Extensions;
using Application.Common.Identity;
using Application.Common.Services.FileService;
using Application.Common.Services.FileService.Models;
using Domain.Product;
using MediatR;

namespace Application.Products.Commands.UpdateProduct;

public class UpdateProductCommandHandler : IRequestHandler<UpdateProductCommand, bool>
{
    private readonly IProductRepository _productRepository;
    private readonly IProductFileUploadService _productFileUploadService;
    private readonly ICurrentUser _currentUser;

    public UpdateProductCommandHandler(IProductRepository productRepository, IProductFileUploadService productFileUploadService,
        ICurrentUser currentUser)
    {
        _productRepository = productRepository;
        _productFileUploadService = productFileUploadService;
        _currentUser = currentUser;
    }

    public async Task<bool> Handle(UpdateProductCommand request, CancellationToken cancellationToken)
    {
        var currentUserId = _currentUser.GetCurrentUserId();

        var product = await _productRepository.GetByIdAsync(request.Id);

        if (product is null)
        {
            throw new NotFoundException($"未找到商品編號 {request.Id} 的商品。請確保您輸入的編號正確，或者商品可能已被刪除。");
        }

        List<MoveFileParam> newImages = new();

        // 更新商品屬性
        product.Name = request.Name;
        product.Description = request.Description;
        product.Brand = request.Brand;
        product.Weight = request.Weight;
        product.Dimensions = request.Dimensions;
        product.UpdatedAt = DateTimeOffset.UtcNow;
        product.UpdatedBy = currentUserId;
        // 刪除商品圖片
        product.Images = product.Images.Where(image => request.OriImageIds.Contains(image.Id.ToString())).ToList();

        // 新增商品圖片
        var newProductImageParams = request.NewImages.Select(image => image.ToMoveFileParam()).ToList();

        newImages.Concat(newProductImageParams);

        foreach (var newProductImageParam in newProductImageParams)
        {
            var productImageExpectedResult = _productFileUploadService.GetMoveProductImageExcpectedResult(newProductImageParam, product.Id);
            product.Images.Add(new ProductImageAttachment
            {
                FileName = productImageExpectedResult.File.FileName,
                OriFileName = productImageExpectedResult.File.OriFileName,
                Path = productImageExpectedResult.File.Path,
                Uri = productImageExpectedResult.File.Uri,
                CreatedAt = DateTimeOffset.Now,
                CreatedBy = currentUserId
            });
        }

        _productRepository.Update(product);
        await _productRepository.SaveChangesAsync(cancellationToken);

        var oriProductItems = request.ProductItems.Where(x => x.Id is not null).ToList();
        var newProductItems = request.ProductItems.Where(x => x.Id is null).ToList();

        // 刪除商品項目
        product.ProductItems = product.ProductItems
            .Where(productItem => oriProductItems.Any(x => x.Id == productItem.Id))
            .ToList();

        // 更新商品項目
        product.ProductItems = product.ProductItems.Select(productItem =>
        {
            var updateProductItem = oriProductItems.First(x => x.Id == productItem.Id);

            productItem.Name = updateProductItem.Name;
            productItem.Price = updateProductItem.Price;
            productItem.StockQuantity = updateProductItem.StockQuantity;
            productItem.IsActive = updateProductItem.IsActive ?? false;
            productItem.UpdatedAt = DateTimeOffset.Now;
            productItem.UpdatedBy = currentUserId;

            if (updateProductItem.OriImageId is null && updateProductItem.NewImage is null)
            {
                productItem.Image = null;
            }
            else if (updateProductItem.NewImage is not null)
            {
                var newProductItemImageParam = updateProductItem.NewImage.ToMoveFileParam();

                newImages.Add(newProductItemImageParam);

                var productItemImageExpectedResult = _productFileUploadService
                    .GetMoveProductImageExcpectedResult(newProductItemImageParam, product.Id);

                productItem.Image = new ProductItemImageAttachment
                {
                    FileName = productItemImageExpectedResult.File.FileName,
                    OriFileName = productItemImageExpectedResult.File.OriFileName,
                    Path = productItemImageExpectedResult.File.Path,
                    Uri = productItemImageExpectedResult.File.Uri,
                    CreatedAt = DateTimeOffset.Now,
                    CreatedBy = currentUserId
                };
            }

            return productItem;
        }).ToList();

        // 新增商品項目
        foreach (var newProductItem in newProductItems)
        {
            ProductItemImageAttachment? newImage = null;

            if (newProductItem.NewImage is not null)
            {
                var newProductItemImageParam = newProductItem.NewImage.ToMoveFileParam();

                newImages.Add(newProductItemImageParam);

                var productItemImageExpectedResult = _productFileUploadService
                    .GetMoveProductImageExcpectedResult(newProductItemImageParam, product.Id);

                newImage = new ProductItemImageAttachment
                {
                    FileName = productItemImageExpectedResult.File.FileName,
                    OriFileName = productItemImageExpectedResult.File.OriFileName,
                    Path = productItemImageExpectedResult.File.Path,
                    Uri = productItemImageExpectedResult.File.Uri,
                    CreatedAt = DateTimeOffset.Now,
                    CreatedBy = currentUserId
                };
            }

            product.ProductItems.Add(new ProductItem
            {
                Name = newProductItem.Name,
                Price = newProductItem.Price,
                StockQuantity = newProductItem.StockQuantity,
                IsActive = newProductItem.IsActive ?? false,
                CreatedAt = DateTimeOffset.Now,
                CreatedBy = currentUserId,
                Image = newImage
            });
        }

        _productRepository.Update(product);
        await _productRepository.SaveChangesAsync(cancellationToken);

        // 最後移動暫存檔案到正式資料夾
        await _productFileUploadService.MoveProductImages(newImages, product.Id);

        return true;
    }
}
