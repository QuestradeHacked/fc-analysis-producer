using GraphQL;
using GraphQL.Client.Abstractions;
using Microsoft.Extensions.Logging;
using Questrade.FinCrime.Analysis.Producer.Domain.Constants;
using Questrade.FinCrime.Analysis.Producer.Domain.Exceptions;
using Questrade.FinCrime.Analysis.Producer.Domain.Models.CRM;
using Questrade.FinCrime.Analysis.Producer.Domain.Repositories.GraphQL;
using SerilogTimings;

namespace Questrade.FinCrime.Analysis.Producer.Infra.Repositories.GraphQL;

public class CustomerRepository : ICustomerRepository
{
    private const string ErrorPathForManage = "domesticaddress";

    private readonly IGraphQLClient _graphQlClient;
    private readonly ILogger<CustomerRepository> _logger;

    public CustomerRepository(
        IGraphQLClient graphQlClient,
        ILogger<CustomerRepository> logger)
    {
        _graphQlClient = graphQlClient;
        _logger = logger;
    }

    public async Task<Tuple<CrmPerson?, PersonAccount?>> GetCustomerInfoAsync(string dataUserId,
        CancellationToken cancellationToken = default)
    {
        string? personIdData = null;

        if (!int.TryParse(dataUserId, out var userId)) throw new ArgumentException("UserId is not valid");

        using (Operation.Time("Fetching Person information from CRM using userId : {userId}.", userId))
        {
            var request = new GraphQLRequest(CrmConstants.GetPersonIdByUserId, new { userId });

            var queryResponse = await _graphQlClient.SendQueryAsync<CrmResponse>(request, cancellationToken);

            if (queryResponse.Errors == null || !queryResponse.Errors.Any())
            {
                var dataSet = queryResponse.Data.UserPerson?.FirstOrDefault()?.Persons!.FirstOrDefault();
                personIdData = dataSet?.PersonId;
            }

            if (queryResponse.Errors != null && queryResponse.Errors.Any())
            {
                if (queryResponse.Errors.Any(error =>
                        error.Path?.Any(path => Convert.ToString(path)?.ToLower() == ErrorPathForManage) == true))
                    _logger.LogDebug("Adding the personId {personIdData} to the hold list.", personIdData);

                throw new CrmException(queryResponse.Errors.Select(error => new CrmException(error.Message)));
            }
        }

        if (!int.TryParse(personIdData, out var personId)) throw new ArgumentException("PersonId is not valid");

        using (Operation.Time("Fetching User information for {UserId} and Person Accounts for {PersonIds} from CRM.",
                   userId, personId))
        {
            var request = new GraphQLRequest(CrmConstants.GetUserAndPersonAccountsQuery, new { userId, personIds = personId });

            var queryResponse = await _graphQlClient.SendQueryAsync<CrmResponse>(request, cancellationToken);

            if (queryResponse.Errors == null || !queryResponse.Errors.Any())
                return Tuple.Create(
                    queryResponse.Data.UserPerson?.FirstOrDefault()?.Persons?.FirstOrDefault(),
                    queryResponse.Data.PersonAccounts?.Where(d => d.EffectiveDate.HasValue)
                        .MaxBy(d => d.EffectiveDate));

            if (queryResponse.Errors.Any(error =>
                    error.Path?.Any(path => Convert.ToString(path)?.ToLower() == ErrorPathForManage) == true))
                _logger.LogDebug("Adding the UserId {userId} to the hold list.", userId);

            throw new CrmException(queryResponse.Errors.Select(error => new CrmException(error.Message)));
        }
    }
}
