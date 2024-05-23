using Newtonsoft.Json;

namespace Questrade.FinCrime.Analysis.Producer.Domain.Models.CRM;

public class CrmResponse
{
    public CrmResponse()
    {
        UserPerson = new List<UserPersons>();
        PersonAccounts = new List<PersonAccount>();
    }

    public IList<PersonAccount>? PersonAccounts { get; set; }

    [JsonProperty("userPerson")]
    public IList<UserPersons>? UserPerson { get; set; }
}
