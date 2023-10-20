namespace Application.Common.Exceptions;

public class FailureException : Exception
{
    public FailureException(string message) : base(message)
    {
    }
}
