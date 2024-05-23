using Newtonsoft.Json;

namespace Questrade.FinCrime.Analysis.Producer.Domain.Models.CRM;

public class InternationalAddress
{
    [JsonProperty("addressLine1")]
    public string AddressLine1 { get; set; } = string.Empty;

    [JsonProperty("addressLine2")]
    public string AddressLine2 { get; set; } = string.Empty;

    [JsonProperty("addressLine3")]
    public string? AddressLine3 { get; set; }

    [JsonProperty("addressTypeId")]
    public string? AddressTypeId { get; set; }

    [JsonProperty("bpsCountryCode")]
    public string? BpsCountryCode { get; set; }

    [JsonProperty("countryCode")]
    public string? CountryCode { get; set; }

    [JsonProperty("countryId")]
    public string? CountryId { get; set; }

    [JsonProperty("countryName")]
    public string CountryName { get; set; } = string.Empty;

    [JsonProperty("isIRSTreatyCountry")]
    public string? IsIRSTreatyCountry { get; set; }

    [JsonProperty("ismCountryCode")]
    public string? IsmCountryCode { get; set; }

    [JsonProperty("ismResidenceCode")]
    public string? IsmResidenceCode { get; set; }

    [JsonProperty("isoCountryCode")]
    public string? IsoCountryCode { get; set; }

    [JsonProperty("postalCode")]
    public string PostalCode { get; set; } = string.Empty;

    [JsonProperty("provinceState")]
    public string ProvinceState { get; set; } = string.Empty;
}
