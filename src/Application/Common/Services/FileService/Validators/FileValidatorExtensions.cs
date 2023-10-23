using FluentValidation;

namespace Application.Common.Services.FileService.Validators;

public static class FileValidatorExtensions
{
    public static IRuleBuilderOptions<T, string> InMimeType<T>(this IRuleBuilder<T, string> ruleBuilder, string[] mimeType)
    {
        return ruleBuilder.Must(value => mimeType.Any(type => value.ToLower() == type.ToLower()))
            .WithMessage("不允許此檔案類型。");
    }

    public static IRuleBuilder<T, long> SizeLessThenMB<T>(this IRuleBuilder<T, long> ruleBuilder, long mb)
    {
        return ruleBuilder.LessThanOrEqualTo(mb * 1024 * 1024)
            .WithMessage($"檔案大小不得超過 {mb}MB。");
    }
}
