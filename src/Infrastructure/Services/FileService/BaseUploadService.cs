using Application.Common.Exceptions;
using Application.Common.Services.FileService;
using Application.Common.Services.FileService.Models;
using Application.Common.Services.FileService.Validators;
using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Specialized;
using Domain.Attachment;
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

    public virtual MoveFileResult GetMoveTmpFileToTargetFolderExcpectedResult(MoveFileParam fileParam, string[] targetFolderPaths)
    {
        MoveFileResult response = new();

        (string sourceFileNameWithPath, string destinationFileNameWithPath) = GetSourceAndDestinationFilePath(fileParam.TmpFileName, fileParam.TargetFileName, targetFolderPaths);

        BlobClient destinationFileClient = _container.GetBlobClient(destinationFileNameWithPath);

        response.IsSucceed = true;
        response.File = new MoveFileDto
        {
            OriFileName = fileParam.TargetFileName,
            FileName = fileParam.TmpFileName,
            Path = destinationFileNameWithPath,
            Uri = destinationFileClient.Uri.AbsoluteUri
        };

        return response;
    }

    public virtual async Task<MoveFileResult> MoveTmpFileToTargetFolder(MoveFileParam fileParam, string[] targetFolderPaths)
    {
        MoveFileResult response = new();

        (string sourceFileNameWithPath, string destinationFileNameWithPath) =
            GetSourceAndDestinationFilePath(fileParam.TmpFileName, fileParam.TargetFileName, targetFolderPaths);

        BlobClient sourceFileClient = _container.GetBlobClient(sourceFileNameWithPath);

        if (!await sourceFileClient.ExistsAsync())
        {
            response.IsSucceed = false;
            response.Message = $"Tmp File {fileParam.TmpFileName} is not exist";
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
            OriFileName = fileParam.TargetFileName,
            FileName = fileParam.TmpFileName,
            Path = destinationFileNameWithPath,
            Uri = destinationFileClient.Uri.AbsoluteUri
        };

        return response;
    }

    /// <summary>
    /// 複製暫存檔案到目標資料夾
    /// TODO: 這裡可以嘗試用 Task.WaitAll () 來實作
    /// </summary>
    /// <param name="filesParam"></param>
    /// <param name="targetFolderPaths"></param>
    /// <returns></returns>
    public virtual async Task<IEnumerable<MoveFileResult>> MoveTmpFilesToTargetFolder(IList<MoveFileParam> filesParam, string[] targetFolderPaths)
    {
        var resultList = new List<MoveFileResult>();

        foreach (var fileParam in filesParam)
        {
            resultList.Add(await MoveTmpFileToTargetFolder(fileParam, targetFolderPaths));
        }

        return resultList;
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

    /// <summary>
    /// 刪除檔案
    /// </summary>
    /// <param name="file"></param>
    /// <returns></returns>
    public Task DeleteFile(Attachment file)
    {
        return DeleteFile(file.Path);
    }

    /// <summary>
    /// 刪除多個檔案
    /// </summary>
    /// <param name="files"></param>
    /// <returns></returns>
    public async Task DeleteFiles(IEnumerable<Attachment> files)
    {
        var checkedFiles = files.Where(file => !string.IsNullOrEmpty(file.Path));

        if (checkedFiles.Any())
        {
            await DeleteFiles(checkedFiles.Select(file => file.Path));
        }
    }

    /// <summary>
    /// 刪除檔案
    /// </summary>
    /// <param name="fileName"></param>
    /// <returns></returns>
    public virtual async Task DeleteFile(string fileName)
    {
        await DeleteFiles(new string[] { fileName });
    }

    /// <summary>
    /// 刪除多個檔案
    /// </summary>
    /// <param name="fileNames"></param>
    /// <returns></returns>
    /// <exception cref="NotImplementedException"></exception>
    public virtual async Task DeleteFiles(IEnumerable<string> fileNames)
    {
        BlobBatchClient batch = _blobServiceClient.GetBlobBatchClient();

        var distinctFileNames = fileNames.Distinct();

        // 檢查檔案是否存在
        var existsTasks = distinctFileNames
            .Select(fileName => _container.GetBlobClient(fileName).ExistsAsync())
            .ToList();

        var fileExistes = await Task.WhenAll(existsTasks);

        // 剔除不存在的檔案
        var blobClients = distinctFileNames
            .Where((file, index) => fileExistes[index].Value)
            .Select(fileName => _container.GetBlobClient(fileName))
            .ToList();

        // TODO: 這個批次刪除有一些限制尚未實作，如檔案數量上限為 256 個，request body 大小 4MB.
        // Ref: (https://learn.microsoft.com/en-us/dotnet/api/overview/azure/storage.blobs.batch-readme?view=azure-dotnet#key-concepts)
        if (blobClients.Any())
        {
            await batch.DeleteBlobsAsync(blobClients.Select(blob => blob.Uri));
        }
    }

    private string GetSourceFilePath(string tmpFileName)
    {
        return string.Join("/", TMP_FOLDER, tmpFileName);
    }

    private (string, string) GetSourceAndDestinationFilePath(string tmpFileName, string oriFileName, string[] targetFolderPaths)
    {
        string sourceFileNameWithPath = GetSourceFilePath(tmpFileName);
        string destinationFileNameWithPath = string.Join("/", targetFolderPaths.Append(tmpFileName));

        return (sourceFileNameWithPath, destinationFileNameWithPath);
    }

}
