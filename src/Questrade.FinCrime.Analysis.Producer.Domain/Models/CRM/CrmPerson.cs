using Newtonsoft.Json;

namespace Questrade.FinCrime.Analysis.Producer.Domain.Models.CRM;

public class CrmPerson
{
    public CrmPerson()
    {
        PhoneNumbers = new List<PhoneNumber>();
    }

    [JsonProperty("addressTypeId")]
    public string? AddressTypeId { get; set; }

    [JsonProperty("created")]
    public string? Created { get; set; }

    [JsonProperty("customerUuid")]
    public string? CustomerUuid { get; set; }

    [JsonProperty("domesticAddress")]
    public DomesticAddress? DomesticAddress { get; set; }

    [JsonProperty("email")]
    public string? Email { get; set; }

    [JsonProperty("firstName")]
    public string FirstName { get; set; } = string.Empty;

    [JsonProperty("internationalAddress")]
    public InternationalAddress? InternationalAddress { get; set; }

    [JsonProperty("lastName")]
    public string LastName { get; set; } = string.Empty;

    [JsonProperty("middleName")]
    public string? MiddleName { get; set; }
    [JsonProperty("personId")]
    public string? PersonId { get; set; }
    [JsonProperty("phoneNumbers")]
    public IList<PhoneNumber> PhoneNumbers { get; set; }

    [JsonProperty("updated")]
    public string? Updated { get; set; }


}
