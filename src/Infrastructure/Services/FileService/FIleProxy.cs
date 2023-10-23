using Application.Common.Services.FileService.Models;
using Microsoft.AspNetCore.Http;

namespace Infrastructure.Services.FileService;

/// <summary>
/// 將 IFormFile 封裝成 IFileData（Proxy 類別）
/// </summary>
public class FIleProxy : IFile
{
    private readonly IFormFile _file;

    public string FileName => _file.FileName;

    public string Extension => Path.GetExtension(_file.FileName);

    public string ContentType => _file.ContentType;

    public long Length => _file.Length;

    public FIleProxy(IFormFile file)
    {
        _file = file ?? throw new ArgumentNullException(nameof(file));
    }

    public Stream OpenReadStream()
    {
        return _file.OpenReadStream();
    }

    public async Task CopyToAsync(Stream target, CancellationToken cancellationToken = default)
    {
        await _file.CopyToAsync(target, cancellationToken);
    }
}
