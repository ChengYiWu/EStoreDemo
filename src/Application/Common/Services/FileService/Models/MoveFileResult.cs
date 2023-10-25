using Application.Common.Models;

namespace Application.Common.Services.FileService.Models;

public class MoveFileResult : Result
{
    public MoveFileDto File { get; set; } = new();
}

public class MoveFileDto
{
    public string OriFileName { get; set; }

    public string FileName { get; set; }

    public string Path { get; set; }

    public string Uri { get; set; }
}

