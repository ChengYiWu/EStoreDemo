using Application.Common.Extensions;
using Application.Common.Share;

namespace Application.Common.Services.FileService.Validators;

public class ProductImageFileValidator : BaseFileValidator
{
    private readonly long MaxImageSizeMB = 2;

    private readonly string[] AllowedMimeType = new[] { 
        MimeTypes.Image.Jpeg,
        MimeTypes.Image.Jpg,
        MimeTypes.Image.Png 
    };

    public ProductImageFileValidator()
    {
        RuleFor(x => x.Length)
            .SizeLessThenMB(MaxImageSizeMB);

        RuleFor(x => x.ContentType)
            .InMimeType(AllowedMimeType);
    }
}
