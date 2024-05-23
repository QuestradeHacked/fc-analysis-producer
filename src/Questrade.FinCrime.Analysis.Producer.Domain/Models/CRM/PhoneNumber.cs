using Newtonsoft.Json;

namespace Questrade.FinCrime.Analysis.Producer.Domain.Models.CRM;

public class PhoneNumber
{
    [JsonProperty("phoneNumber")]
    public string? Number { get; set; }

    [JsonProperty("phoneNumberRank")]
    public string? Rank { get; set; }

    [JsonProperty("phoneNumberType")]
    public string? Type { get; set; }

    [JsonProperty("phoneNumberTypeId")]
    public string? TypeId { get; set; }
}
