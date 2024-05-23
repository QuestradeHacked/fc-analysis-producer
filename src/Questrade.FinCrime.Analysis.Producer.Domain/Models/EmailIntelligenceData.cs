using System.Text.Json.Serialization;

namespace Questrade.FinCrime.Analysis.Producer.Domain.Models;

public class EmailIntelligenceData
{
    [JsonPropertyName("accountNumber")] public string? AccountNumber { get; set; }

    [JsonPropertyName("accountStatusId")] public int? AccountStatusId { get; set; }

    [JsonPropertyName("crmUserId")] public string? CrmUserId { get; set; }

    [JsonPropertyName("city")] public string City { get; set; } = string.Empty;

    [JsonPropertyName("country")] public string Country { get; set; } = string.Empty;

    [JsonPropertyName("effectiveDate")] public DateTime? EffectiveDate { get; set; }

    [JsonPropertyName("email")] public string Email { get; set; } = string.Empty;

    [JsonPropertyName("enterpriseProfileId")]
    public string? EnterpriseProfileId { get; set; }

    [JsonPropertyName("firstName")] public string FirstName { get; set; } = string.Empty;

    [JsonPropertyName("lastName")] public string LastName { get; set; } = string.Empty;

    [JsonPropertyName("phoneNumber")] public string PhoneNumber { get; set; } = string.Empty;

    [JsonPropertyName("profileId")] public string? ProfileId { get; set; }

    [JsonPropertyName("state")] public string State { get; set; } = string.Empty;

    [JsonPropertyName("street")] public string Street { get; set; } = string.Empty;

    [JsonPropertyName("zipCode")] public string ZipCode { get; set; } = string.Empty;
}
