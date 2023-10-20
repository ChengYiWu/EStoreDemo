using Application.Common.Exceptions;
using MediatR;

namespace Application.Products.Commands.CreateProduct;

/// <summary>
/// 建立商品
/// </summary>
public sealed class CreateProductCommandHandler : IRequestHandler<CreateProductCommand, int>
{
    public CreateProductCommandHandler()
    {
    }

    /// <summary>
    /// 一個建立商品的 Command Handler 範例
    /// </summary>
    /// <param name="request"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    /// <exception cref="ValidationException"></exception>
    public Task<int> Handle(CreateProductCommand request, CancellationToken cancellationToken)
    {
        // do some business logic ...


        return Task.FromResult(1);
    }
}
