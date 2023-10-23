using Application.Common.Models;

namespace Application.Common.Services.FileService.Models;

public class UploadFileResult : Result
{
    public UploadFileDto? File { get; set; }
}

public class UploadFileDto
{
    public string UniqId { get; set; }

    public string OriFileName { get; set; }

    public string FileName { get; set; }

    public string ContentType { get; set; }

    public string Path { get; set; }

    public string Uri { get; set; }
}

