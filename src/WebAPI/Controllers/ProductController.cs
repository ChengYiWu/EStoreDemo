using Application.Common.Models;
using Application.Products.Commands.CreateProduct;
using Application.Products.Commands.UploadProductImage;
using Application.Products.Queries.GetProduct;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WebAPI.Extensions;

namespace WebAPI.Controllers;

[Route("api/products")]
[ApiController]
public class ProductController : ControllerBase
{
    private readonly ISender _sender;

    public ProductController(ISender sender)
    {
        _sender = sender;
    }

    /// <summary>
    /// 範例，新增商品 API
    /// </summary>
    /// <remarks>
    /// 商品圖片已先上傳至 tmp 資料夾，並取得檔案名稱，再將檔案名稱傳入此 API，內部會將檔案移動至正式資料夾
    /// </remarks>
    /// <param name="command"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    [Authorize]
    [HttpPost]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<int> CreateProduct(CreateProductCommand command, CancellationToken cancellationToken)
    {
        return await _sender.Send(command, cancellationToken);
    }

    /// <summary>
    /// 範例，商品圖片上傳 API
    /// </summary>
    /// <remarks>
    /// 採分別上傳方式，先上傳圖片，再將圖片名稱傳入商品建立 API
    /// </remarks>
    /// <param name="file"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    [HttpPost]
    [Route("upload")]
    public async Task<UploadFileResponse> UploadProductImage(IFormFile file, CancellationToken cancellationToken)
    {
        var command = new UploadProductImageCommand(file.ToFileProxy());
        return await _sender.Send(command, cancellationToken);
    }

    [HttpGet]
    [Route("{id:int}")]
    public async Task<ProductResponse> GetProduct(int id, CancellationToken cancellationToken)
    {
        var query = new GetProductQuery(id);
        return await _sender.Send(query, cancellationToken);
    }
}
