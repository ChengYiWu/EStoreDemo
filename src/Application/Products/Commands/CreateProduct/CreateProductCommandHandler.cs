using Application.Common.Extensions;
using Application.Common.Identity;
using Application.Common.Services.FileService;
using Domain.Product;
using MediatR;

namespace Application.Products.Commands.CreateProduct;

/// <summary>
/// 建立商品
/// </summary>
public sealed class CreateProductCommandHandler : IRequestHandler<CreateProductCommand, int>
{
    private readonly IProductRepository _productRepository;
    private readonly IProductFileUploadService _productFileUploadService;
    private readonly ICurrentUser _currentUser;

    public CreateProductCommandHandler(
        IProductRepository productRepository,
        IProductFileUploadService productFileUploadService,
        ICurrentUser currentUser
        )
    {
        _productRepository = productRepository;
        _productFileUploadService = productFileUploadService;
        _currentUser = currentUser;
    }

    /// <summary>
    /// 建立商品
    /// </summary>
    /// <param name="request"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    public async Task<int> Handle(CreateProductCommand request, CancellationToken cancellationToken)
    {
        var currentUserId = _currentUser.GetCurrentUserId();

        // 建立商品資料與商品項目資料，以取得商品 Id
        var newProductItems = request.ProductItems.Select(productItem =>
        {
            return new ProductItem
            {
                Name = productItem.Name,
                Price = productItem.Price,
                StockQuantity = productItem.StockQuantity,
                IsActive = productItem.IsActive ?? false,
                CreatedAt = DateTimeOffset.UtcNow,
                CreatedBy = currentUserId
            };
        }).ToList();

        Product newProduct = new()
        {
            Name = request.Name,
            Description = request.Description,
            Brand = request.Brand,
            Weight = request.Weight,
            Dimensions = request.Dimensions,
            CreatedAt = DateTimeOffset.UtcNow,
            CreatedBy = currentUserId,
            ProductItems = newProductItems
        };

        _productRepository.Add(newProduct);
        await _productRepository.SaveChangesAsync(cancellationToken);

        // 取得商品 Id 後，新增商品相關圖片資料（DB）
        var productImageParams = request.NewImages.Select(image => image.ToMoveFileParam()).ToList();

        var productImages = productImageParams.Select(imageParam =>
        {
            var productImageExpectedResult = _productFileUploadService.GetMoveProductImageExcpectedResult(imageParam, newProduct.Id);

            return new ProductImageAttachment
            {
                FileName = productImageExpectedResult.File.FileName,
                OriFileName = productImageExpectedResult.File.OriFileName,
                Path = productImageExpectedResult.File.Path,
                Uri = productImageExpectedResult.File.Uri,
                CreatedAt = DateTimeOffset.Now,
                CreatedBy = currentUserId
            };
        }).ToList();

        newProduct.Images = productImages;

        var productItemImageParams = request.ProductItems.Select((productItem) => productItem.NewImage?.ToMoveFileParam()).ToList();

        newProduct.ProductItems = newProduct.ProductItems.Select((productItem, index) =>
        {
            var imageParam = productItemImageParams[index];

            if (imageParam is not null)
            {
                var productItemImageExpectedResult = _productFileUploadService.GetMoveProductImageExcpectedResult(imageParam, newProduct.Id);

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

        _productRepository.Update(newProduct);
        await _productRepository.SaveChangesAsync(cancellationToken);

        // 最後移動暫存檔案到正式資料夾
        var allProductImageParams = productImageParams
            .Concat(productItemImageParams.Where(param => param is not null).Select(param => param!))
            .ToList();

        await _productFileUploadService.MoveProductImages(allProductImageParams, newProduct.Id);

        return newProduct.Id;
    }
}
