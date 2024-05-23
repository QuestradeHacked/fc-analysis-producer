using Newtonsoft.Json;

namespace Questrade.FinCrime.Analysis.Producer.Domain.Models.CRM;

public class UserPersons
{
    public UserPersons()
    {
        Persons = new List<CrmPerson>();
    }

    [JsonProperty("person")]
    public List<CrmPerson>? Persons { get; set; }
}
