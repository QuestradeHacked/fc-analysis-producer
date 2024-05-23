using Newtonsoft.Json;

namespace Questrade.FinCrime.Analysis.Producer.Domain.Models.CRM;

public class PersonAccount
{
    [JsonProperty("accountNumber")]
    public string? AccountNumber { get; set; }

    [JsonProperty("accountStatusId")]
    public int? AccountStatusId { get; set; }

    [JsonProperty("effectiveDate")]
    public DateTime? EffectiveDate { get; set; }
}
