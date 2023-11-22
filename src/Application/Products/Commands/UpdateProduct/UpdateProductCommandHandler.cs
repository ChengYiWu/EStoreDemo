using Application.Common.Exceptions;
using Application.Common.Extensions;
using Application.Common.Identity;
using Application.Common.Services.FileService;
using Application.Common.Services.FileService.Models;
using Domain.Attachment;
using Domain.Product;
using MediatR;

namespace Application.Products.Commands.UpdateProduct;

/// <summary>
/// TODO:
///     1. 已有訂單產生的 ProductItem 不可刪除，但可以停用
/// </summary>
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

        if (product.IsEditable.HasValue && !product.IsEditable.Value)
        {
            throw new FailureException("不可變更資料。");
        }

        var newImages = new List<MoveFileParam>();
        var oldImages = new List<Attachment>();

        // 更新商品屬性
        product.Name = request.Name;
        product.Description = request.Description;
        product.Brand = request.Brand;
        product.Weight = request.Weight;
        product.Dimensions = request.Dimensions;
        product.UpdatedAt = DateTimeOffset.UtcNow;
        product.UpdatedBy = currentUserId;

        var retainedProductImages = product.Images.Where(image => request.OriImageIds.Contains(image.Id.ToString())).ToList();
        var deletedProductImages = product.Images.Except(retainedProductImages).ToList();

        product.Images = retainedProductImages;

        // 紀錄要刪除的商品圖片
        oldImages.AddRange(deletedProductImages);

        // 新增商品圖片
        var newProductImageParams = request.NewImages.Select(image => image.ToMoveFileParam()).ToList();

        newImages.AddRange(newProductImageParams);

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

        var oriProductItems = request.ProductItems.Where(x => x.Id is not null).ToList();
        var newProductItems = request.ProductItems.Except(oriProductItems).ToList();

        // 新增或刪除商品項目
        var retainedProductItems = product.ProductItems
            .Where(productItem => oriProductItems.Any(x => x.Id == productItem.Id))
            .ToList();
        var deletedProductItems = product.ProductItems.Except(retainedProductItems).ToList();

        // 紀錄要刪除的商品圖片
        oldImages.AddRange(deletedProductItems.Where(x => x.Image is not null).Select(x => x.Image!));

        // 更新商品項目
        product.ProductItems = retainedProductItems.Select(productItem =>
        {
            var updateProductItem = oriProductItems.First(x => x.Id == productItem.Id);

            productItem.Name = updateProductItem.Name;
            productItem.Price = updateProductItem.Price;
            productItem.StockQuantity = updateProductItem.StockQuantity;
            productItem.IsActive = updateProductItem.IsActive ?? false;
            productItem.UpdatedAt = DateTimeOffset.Now;
            productItem.UpdatedBy = currentUserId;

            var isExistImage = productItem.Image is not null;

            if (updateProductItem.OriImageId is null && updateProductItem.NewImage is null)
            {
                if (isExistImage)
                {
                    oldImages.Add(productItem.Image!);
                }

                productItem.Image = null;
            }
            else if (updateProductItem.NewImage is not null)
            {
                var newProductItemImageParam = updateProductItem.NewImage.ToMoveFileParam();

                newImages.Add(newProductItemImageParam);

                var productItemImageExpectedResult = _productFileUploadService
                    .GetMoveProductImageExcpectedResult(newProductItemImageParam, product.Id);

                if (isExistImage)
                {
                    oldImages.Add(productItem.Image!);
                }

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

        // 新圖片搬移
        await _productFileUploadService.MoveProductImages(newImages, product.Id);

        // 舊圖片刪除
        await _productFileUploadService.DeleteFiles(oldImages);

        return true;
    }
}
