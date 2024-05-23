using Questrade.FinCrime.Analysis.Producer.Domain.Models.CRM;

namespace Questrade.FinCrime.Analysis.Producer.Domain.Repositories.GraphQL;

public interface ICustomerRepository
{
    Task<Tuple<CrmPerson?, PersonAccount?>> GetCustomerInfoAsync(string userId,
        CancellationToken cancellationToken = default);
}
