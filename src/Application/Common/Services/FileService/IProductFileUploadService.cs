using Application.Common.Services.FileService.Models;

namespace Application.Common.Services.FileService;

public interface IProductFileUploadService : IUploadService
{
    public Task<UploadFileResult> UploadProductImage(IFile file);

    public Task<MoveFileResult> MoveProductImage(MoveFileParam fileParam, int productId);

    public Task<IEnumerable<MoveFileResult>> MoveProductImages(IList<MoveFileParam> filesParam, int productId);

    public MoveFileResult GetMoveProductImageExcpectedResult(MoveFileParam fileParam, int productId);
}
