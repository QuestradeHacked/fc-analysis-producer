namespace Questrade.FinCrime.Analysis.Producer.Domain.Exceptions;

public class NoAddressFoundException : BusinessException
{
    public NoAddressFoundException()
        : base("No address was found")
    {
    }
}
