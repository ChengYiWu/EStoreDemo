using Application.Common.Services.FileService.Models;

namespace Application.Common.Services.FileService;

public interface IProductFileUploadService : IUploadService
{
    Task<UploadFileResult> UploadProductImage(IFile file);

    Task<MoveFileResult> MoveProductImage(string tmpFileName, string targetFileName, string productId);

    public MoveFileResult GetMoveProductImageResult(string tmpFileName, string targetFileName, string productId);
}
