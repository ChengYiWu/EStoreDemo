using Infrastructure.Services.FileService;

namespace WebAPI.Extensions;

public static class IFormFileExtensions
{
    public static FIleProxy ToFileProxy(this IFormFile formData)
    {
        return new FIleProxy(formData);
    }
}
