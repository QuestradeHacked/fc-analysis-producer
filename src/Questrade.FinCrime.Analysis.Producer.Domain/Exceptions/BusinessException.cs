namespace Questrade.FinCrime.Analysis.Producer.Domain.Exceptions;

public abstract class BusinessException : AggregateException
{
    protected BusinessException(string? message)
        : base(message)
    {
    }

    protected BusinessException(string? message, IEnumerable<Exception> exceptions)
        : base(message, exceptions)
    {
    }
}
