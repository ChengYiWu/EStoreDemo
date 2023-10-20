using FluentValidation.Results;

namespace Application.Common.Exceptions;

public class ValidationException : Exception
{
    public IDictionary<string, string[]> Errors { get; }

    public ValidationException() : base("發生了一個或多個驗證失敗。")
    {
        Errors = new Dictionary<string, string[]>();
    }

    public ValidationException(IEnumerable<ValidationFailure> failures)
        : this()
    {
        Errors = failures
            .GroupBy(e => e.PropertyName, e => e.ErrorMessage)
            .ToDictionary(failureGroup => failureGroup.Key, failureGroup => failureGroup.ToArray());
    }

    public ValidationException(string propertyName, string errorMessage)
        : this()
    {
        Errors.Add(propertyName, new[] { errorMessage });
    }
}
