using ValidationException = Application.Common.Exceptions.ValidationException;

using FluentValidation;
using MediatR;

namespace Application.Common.Behaviours;

/// <summary>
/// 透過 MediatR 的 Pipeline Behaviour 來驗證 Request
/// </summary>
/// <typeparam name="TRequest"></typeparam>
/// <typeparam name="TResponse"></typeparam>
public class ValidationBecaviour<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
    where TRequest : notnull
{
    private readonly IEnumerable<IValidator<TRequest>> _validators;

    public ValidationBecaviour(IEnumerable<IValidator<TRequest>> validators)
    {
        _validators = validators;
    }

    public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
    {
        if (_validators.Any())
        {
            var context = new ValidationContext<TRequest>(request);

            var validationResults = await Task.WhenAll(
               _validators.Select(v =>
                   v.ValidateAsync(context, cancellationToken))
            );

            var failures = validationResults
                   .Where(r => r.Errors.Any())
                   .SelectMany(r => r.Errors)
                   .ToList();

            if (failures.Any())
                throw new ValidationException(failures);
        }

        return await next();
    }
}
