using Application.Common.Services.FileService.Models;
using FluentValidation;

namespace Application.Common.Services.FileService.Validators;

public class BaseFileValidator : AbstractValidator<IFile>
{
    public BaseFileValidator()
    {
        RuleFor(x => x.Length)
            .GreaterThan(0).WithMessage("不允許上傳空檔案。");
    }
}
