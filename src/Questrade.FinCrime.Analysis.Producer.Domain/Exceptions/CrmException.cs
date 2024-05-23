namespace Questrade.FinCrime.Analysis.Producer.Domain.Exceptions;

public class CrmException : BusinessException
{
    public CrmException(string? message)
        : base(message)
    {
    }

    public CrmException(IEnumerable<Exception> exceptions) :
        base("Error when retrieving user from Crm.", exceptions)
    {
    }
}
