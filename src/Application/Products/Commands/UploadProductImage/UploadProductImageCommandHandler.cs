using Application.Common.Exceptions;
using Application.Common.Models;
using Application.Common.Services.FileService;
using MediatR;

namespace Application.Products.Commands.UploadProductImage;

public class UploadProductImageCommandHandler : IRequestHandler<UploadProductImageCommand, UploadFileResponse>
{
    private readonly IProductFileUploadService _productFileUploadService;

    public UploadProductImageCommandHandler(IProductFileUploadService productFileUploadService)
    {
        _productFileUploadService = productFileUploadService;
    }

    public async Task<UploadFileResponse> Handle(UploadProductImageCommand request, CancellationToken cancellationToken)
    {
        var result = await _productFileUploadService.UploadProductImage(request.File);

        if (!result.IsSucceed || result.File is null)
        {
            throw new FailureException(result.Message);
        }

        var file = result.File;

        return new UploadFileResponse
        {
            UniqId = file.UniqId,
            OriFileName = file.OriFileName,
            FileName = file.FileName,
            Uri = file.Uri
        };
    }
}
