using Application.Common.Services.FileService;
using Application.Common.Services.FileService.Models;
using Application.Common.Services.FileService.Validators;
using Infrastructure.Options;
using Microsoft.Extensions.Options;

namespace Infrastructure.Services.FileService;

public class ProductFileUploadService : BaseUploadService, IProductFileUploadService
{
    /// <summary>
    /// 商品檔案根目錄
    /// </summary>
    private const string PRODUCT_FILE_ROOT_FOLDER = "product";

    /// <summary>
    /// 商品暫存檔案群組
    /// </summary>
    public ProductFileUploadService(IOptions<FileStorageOption> option) : base(option)
    {
    }

    public async Task<UploadFileResult> UploadProductImage(IFile file)
    {
        ValidateFileAndThrowIfNotValid(file, new ProductImageFileValidator());

        return await UploadFileToTmpFolder(file);
    }

    public async Task<MoveFileResult> MoveProductImage(string tmpFileName, string oriFileName, string productId)
    {
        var response = await base.MoveTmpFileToTargetFolder(
            tmpFileName,
            oriFileName,
            new string[] { PRODUCT_FILE_ROOT_FOLDER, productId }
        );

        return response;
    }

    public MoveFileResult GetMoveProductImageResult(string tmpFileName, string targetFileName, string productId)
    {
        return base.GetMoveTmpFileToTargetFolderResult(
            tmpFileName,
            targetFileName,
            new string[] { PRODUCT_FILE_ROOT_FOLDER, productId }
        );
    }
}
