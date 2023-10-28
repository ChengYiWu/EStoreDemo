using FluentValidation;

namespace Application.Common.Extensions;

public static class ValidatorExtensions
{
    public static IRuleBuilderOptions<T, bool?> NullOrBoolean<T>(this IRuleBuilder<T, bool?> ruleBuilder)
    {
        return ruleBuilder.Must(a => a == null || (a == false || a == true)).WithMessage("請傳入 true 或 false");
    }

    public static IRuleBuilderOptions<T, string?> NullOrNotWhitespace<T>(this IRuleBuilder<T, string?> ruleBuilder)
    {
        return ruleBuilder.Must(value => value == null || !string.IsNullOrWhiteSpace(value))
            .WithMessage("不可輸入空字串。");
    }

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
