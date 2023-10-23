using Application.Common.Services.FileService.Models;

namespace Application.Common.Services.FileService;

public interface IUploadService
{
    /// <summary>
    /// 上傳檔案
    /// </summary>
    /// <param name="file">檔案資訊</param>
    /// <returns></returns>
    public Task<UploadFileResult> UploadFile(IFile file);

    /// <summary>
    /// 上傳檔案並指定存放的特定路徑
    /// </summary>
    /// <param name="file"></param>
    /// <param name="paths"></param>
    /// <returns></returns>
    public Task<UploadFileResult> UploadFile(IFile file, string[] paths);

    /// <summary>
    /// 上傳檔案到暫存資料夾
    /// </summary>
    /// <param name="file">檔案資訊</param>
    /// <returns></returns>
    public Task<UploadFileResult> UploadFileToTmpFolder(IFile file);

    /// <summary>
    /// 實際的檔案移動暫存檔案到指定資料夾
    /// </summary>
    /// <param name="tmpFileName">暫存檔案名稱</param>
    /// <param name="targetFileName">指定檔案名稱</param>
    /// <param name="targetFolderPaths">指定檔案路徑</param>
    /// <returns></returns>
    public Task<MoveFileResult> MoveTmpFileToTargetFolder(string tmpFileName, string targetFileName, string[] targetFolderPaths);

    /// <summary>
    /// 預先取得存檔案移動到指定資料夾的結果，尚未執行實際的檔案移動
    /// </summary>
    /// <param name="tmpFileName">暫存檔案名稱</param>
    /// <param name="targetFileName">指定檔案名稱</param>
    /// <param name="targetFolderPaths">指定檔案路徑</param>
    /// <returns></returns>
    public MoveFileResult GetMoveTmpFileToTargetFolderResult(string tmpFileName, string targetFileName, string[] targetFolderPaths);

    /// <summary>
    /// 將相對路徑轉換為絕對路徑
    /// </summary>
    /// <param name="relativePath">相對路徑</param>
    /// <returns>絕對路徑</returns>
    public string FromRelativePathToAbsoluteUri(string relativePath);
}
