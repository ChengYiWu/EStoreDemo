using Application.Common.Exceptions;
using Application.Common.Models;
using Application.Common.Services.FileService;
using Application.Common.Services.FileService.Models;
using Application.Common.Services.FileService.Validators;
using Azure.Storage.Blobs;
using FluentValidation;
using Infrastructure.Options;
using Microsoft.Extensions.Options;

namespace Infrastructure.Services.FileService;

public class BaseUploadService : IUploadService
{
    private readonly FileStorageOption _option;
    private readonly BlobServiceClient _blobServiceClient;
    private readonly BlobContainerClient _container;

    /// <summary>
    /// 暫存檔案目錄內的所有檔案都會被定期清除（於 Azure Storage 上設定「生命週期管理」，將 tmp 目錄中已建立超過一天的檔案刪除）
    /// </summary>
    private const string TMP_FOLDER = "tmp";

    public BaseUploadService(IOptions<FileStorageOption> option)
    {
        _option = option.Value;
        _blobServiceClient = new BlobServiceClient(_option.ConnectionString);
        _container = _blobServiceClient.GetBlobContainerClient(_option.Container);
    }

    public virtual Task<UploadFileResult> UploadFileToTmpFolder(IFile file)
    {
        return UploadFile(file, new[] { TMP_FOLDER });
    }

    public virtual Task<UploadFileResult> UploadFile(IFile file)
    {
        return UploadFile(file, Array.Empty<string>());
    }

    public virtual async Task<UploadFileResult> UploadFile(IFile file, string[] paths)
    {
        ValidateFileAndThrowIfNotValid(file, new BaseFileValidator());

        UploadFileResult response = new();

        string uniqId = Guid.NewGuid().ToString();
        string blobFileName = uniqId + file.Extension;
        string fullBlobFileName = string.Join("/", paths.Append(blobFileName));

        BlobClient client = _container.GetBlobClient(fullBlobFileName);
        await using (Stream? data = file.OpenReadStream())
        {
            await _container.UploadBlobAsync(fullBlobFileName, data);
        }

        UploadFileDto fileDto = new()
        {
            UniqId = uniqId,
            OriFileName = file.FileName,
            FileName = blobFileName,
            ContentType = file.ContentType,
            Path = fullBlobFileName,
            Uri = client.Uri.AbsoluteUri
        };

        response.Message = $"檔案 {fileDto.OriFileName} 上傳成功。";
        response.IsSucceed = true;
        response.File = fileDto;

        return response;
    }

    public virtual MoveFileResult GetMoveTmpFileToTargetFolderResult(string tmpFileName, string oriFileName, string[] targetFolderPaths)
    {
        MoveFileResult response = new();

        (string sourceFileNameWithPath, string destinationFileNameWithPath) = GetSourceAndDestinationFilePath(tmpFileName, oriFileName, targetFolderPaths);

        BlobClient destinationFileClient = _container.GetBlobClient(destinationFileNameWithPath);

        response.IsSucceed = true;
        response.File = new MoveFileDto
        {
            OriFileName = oriFileName,
            FileName = tmpFileName,
            Path = destinationFileNameWithPath,
            Uri = destinationFileClient.Uri.AbsoluteUri
        };

        return response;
    }

    public virtual async Task<MoveFileResult> MoveTmpFileToTargetFolder(string tmpFileName, string oriFileName, string[] targetFolderPaths)
    {
        MoveFileResult response = new();

        (string sourceFileNameWithPath, string destinationFileNameWithPath) =
            GetSourceAndDestinationFilePath(tmpFileName, oriFileName, targetFolderPaths);

        BlobClient sourceFileClient = _container.GetBlobClient(sourceFileNameWithPath);

        if (!await sourceFileClient.ExistsAsync())
        {
            response.IsSucceed = false;
            response.Message = $"Tmp File {tmpFileName} is not exist";
            return response;
        }

        BlobClient destinationFileClient = _container.GetBlobClient(destinationFileNameWithPath);

        var copyOperation = await destinationFileClient.StartCopyFromUriAsync(sourceFileClient.Uri);
        await copyOperation.WaitForCompletionAsync();

        // 不需刪除暫存目錄下的檔案，Azure Storage 會定期自動刪除
        //await sourceFileClient.DeleteIfExistsAsync();

        response.IsSucceed = true;
        response.Message = $"Tmp File {sourceFileNameWithPath} is moved to {destinationFileNameWithPath} successfuly.";
        response.File = new MoveFileDto
        {
            OriFileName = oriFileName,
            FileName = tmpFileName,
            Path = destinationFileNameWithPath,
            Uri = destinationFileClient.Uri.AbsoluteUri
        };

        return response;
    }

    /// <summary>
    /// 驗證上傳檔案並取得錯誤訊息
    /// </summary>
    /// <param name="file">上傳檔案</param>
    /// <param name="validator">驗證器</param>
    /// <returns></returns>
    protected void ValidateFileAndThrowIfNotValid(IFile file, AbstractValidator<IFile> validator)
    {
        var validationResult = validator.Validate(file);

        if (!validationResult.IsValid)
        {
            throw new FailureException(validationResult.Errors.First().ErrorMessage);
        }
    }

    /// <summary>
    /// 從檔案的 container 出發，轉換相對路徑為絕對路徑
    /// </summary>
    /// <param name="relativePath">相對路徑</param>
    /// <returns></returns>
    public virtual string FromRelativePathToAbsoluteUri(string relativePath)
    {
        return _container.GetBlobClient(relativePath).Uri.AbsoluteUri;
    }

    private (string, string) GetSourceAndDestinationFilePath(string tmpFileName, string oriFileName, string[] targetFolderPaths)
    {
        string sourceFileNameWithPath = string.Join("/", TMP_FOLDER, tmpFileName);
        string destinationFileNameWithPath = string.Join("/", targetFolderPaths.Append(tmpFileName));

        return (sourceFileNameWithPath, destinationFileNameWithPath);
    }


}
