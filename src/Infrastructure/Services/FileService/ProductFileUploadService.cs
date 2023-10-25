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

    public async Task<MoveFileResult> MoveProductImage(MoveFileParam fileParam, int productId)
    {
        return await base.MoveTmpFileToTargetFolder(fileParam, GetProductImageFolderPath(productId));

    }

    public async Task<IEnumerable<MoveFileResult>> MoveProductImages(IList<MoveFileParam> filesParam, int productId)
    {
        return await base.MoveTmpFilesToTargetFolder(filesParam, GetProductImageFolderPath(productId));
    }

    public MoveFileResult GetMoveProductImageExcpectedResult(MoveFileParam fileParam, int productId)
    {
        return base.GetMoveTmpFileToTargetFolderExcpectedResult(fileParam, GetProductImageFolderPath(productId));
    }

    private static string[] GetProductImageFolderPath(int productId)
    {
        return new string[] { PRODUCT_FILE_ROOT_FOLDER, productId.ToString() };
    }
}
