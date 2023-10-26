using FluentValidation;

namespace Application.Common.Models.Commands;

public class UploadedFileDTO
{
    public string OriFileName { get; set; } = string.Empty;

    public string TmpFileName { get; set; } = string.Empty;
}


public class UploadedFileDTOValidator : AbstractValidator<UploadedFileDTO>
{
    public UploadedFileDTOValidator()
    {
        RuleFor(x => x.OriFileName)
            .NotNull()
            .NotEmpty();

        RuleFor(x => x.TmpFileName)
            .NotNull()
            .NotEmpty();
    }
}