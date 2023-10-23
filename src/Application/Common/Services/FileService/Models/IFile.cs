namespace Application.Common.Services.FileService.Models;

public interface IFile
{
    /// <summary>
    /// 檔案名稱
    /// </summary>
    string FileName { get; }

    /// <summary>
    /// 副檔名
    /// </summary>
    string Extension { get; }

    /// <summary>
    /// 檔案 Mime Type
    /// </summary>
    string ContentType { get; }

    /// <summary>
    /// 檔案大小
    /// </summary>
    long Length { get; }

    Stream OpenReadStream();

    /// <summary>
    /// 複製檔案到目標 Stream
    /// </summary>
    /// <param name="target">要複製的目標 Stream</param>
    /// <param name="cancellationToken">取消操作 token</param>
    /// <returns></returns>
    Task CopyToAsync(Stream target, CancellationToken cancellationToken);
}