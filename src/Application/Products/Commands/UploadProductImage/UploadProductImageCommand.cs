using Application.Common.Models;
using Application.Common.Services.FileService.Models;
using MediatR;

namespace Application.Products.Commands.UploadProductImage;

public record UploadProductImageCommand(
    IFile File
) : IRequest<UploadFileResponse>;

