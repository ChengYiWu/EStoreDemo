using Application.Common.Exceptions;
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
    /// 一個建立商品的 Command Handler 範例（含圖片上傳）
    /// </summary>
    /// <param name="request"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    /// <exception cref="ValidationException"></exception>
    public async Task<int> Handle(CreateProductCommand request, CancellationToken cancellationToken)
    {
        var currentUserId = _currentUser.GetCurrentUserId();

        Product newProduct = new()
        {
            Name = request.Name,
            Description = request.Description,
        };

        _productRepository.Add(newProduct);
        await _productRepository.SaveChangesAsync(cancellationToken);
        
        // 先取得檔案移動的結果，寫入資料庫，將移動檔案步驟放在最後，避免資料庫寫入失敗，檔案卻已經移動。
        var imageResult = _productFileUploadService.GetMoveProductImageResult(request.ImageTmpFileName, request.ImageOriFileName, newProduct.Id.ToString());

        var imageFile = imageResult.File;
        newProduct.Images.Add(new ProductImageAttachment
        {
            FileName = imageFile?.FileName ?? "",
            OriFileName = imageFile?.OriFileName ?? "",
            Path = imageFile?.Path ?? "",
            Uri = imageFile?.Uri ?? "",
            CreatedAt = DateTimeOffset.UtcNow,
            CreatedBy = currentUserId
        });
        _productRepository.Update(newProduct);
        await _productRepository.SaveChangesAsync(cancellationToken);

        var result = await _productFileUploadService.MoveProductImage(request.ImageTmpFileName, request.ImageOriFileName, newProduct.Id.ToString());
        if (!result.IsSucceed || result.File is null)
        {
            throw new FailureException(imageResult.Message);
        }

        return newProduct.Id;
    }
}
