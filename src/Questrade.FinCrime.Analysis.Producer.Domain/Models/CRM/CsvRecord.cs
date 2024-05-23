using Newtonsoft.Json;

namespace Questrade.FinCrime.Analysis.Producer.Domain.Models.CRM;

public class CsvRecord
{
    [JsonProperty("userId")]
    public string? UserID { get; set; }
}
