using Newtonsoft.Json;

namespace Questrade.FinCrime.Analysis.Producer.Domain.Models.CRM;

public class DomesticAddress
{
    [JsonProperty("city")]
    public string City { get; set; } = string.Empty;

    [JsonProperty("postalCode")]
    public string PostalCode { get; set; } = string.Empty;

    [JsonProperty("province")]
    public string Province { get; set; } = string.Empty;

    [JsonProperty("provinceCode")]
    public string ProvinceCode { get; set; } = string.Empty;

    [JsonProperty("provinceId")]
    public string ProvinceId { get; set; } = string.Empty;

    [JsonProperty("streetDirection")]
    public string StreetDirection { get; set; } = string.Empty;

    [JsonProperty("streetName")]
    public string StreetName { get; set; } = string.Empty;

    [JsonProperty("streetNumber")]
    public string StreetNumber { get; set; } = string.Empty;

    [JsonProperty("streetSuffix")]
    public string StreetSuffix { get; set; } = string.Empty;

    [JsonProperty("streetType")]
    public string StreetType { get; set; } = string.Empty;

    [JsonProperty("unitNumber")]
    public string UnitNumber { get; set; } = string.Empty;
}
