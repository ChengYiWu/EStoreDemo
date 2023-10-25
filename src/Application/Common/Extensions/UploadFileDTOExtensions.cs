using Application.Common.Models.Commands;
using Application.Common.Services.FileService.Models;

namespace Application.Common.Extensions;

public static class UploadFileDTOExtensions
{
    public static MoveFileParam ToMoveFileParam(this UploadedFileDTO dto)
    {
        return new MoveFileParam(dto.TmpFileName, dto.OriFileName);
    }
}
