namespace Application.Common.Services.FileService.Models;

public record MoveFileParam(
    string TmpFileName,
    string TargetFileName
);
