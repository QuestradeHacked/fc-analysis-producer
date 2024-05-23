using System.Text.Json.Serialization;
using Questrade.FinCrime.Analysis.Producer.Domain.Constants;
using Questrade.Library.PubSubClientHelper.Subscriber.Default.Models;

namespace Questrade.FinCrime.Analysis.Producer.Domain.Models;

public class EmailIntelligencePublisherMessage
{
    public EmailIntelligencePublisherMessage()
    {
        Id = Guid.NewGuid().ToString();
        Source = SourceNames.Source;
        PublishTime = DateTime.Now;
        Data = new EmailIntelligenceData();
    }

    [JsonPropertyName("messageId")] public string? Id { get; set; }

    [JsonPropertyName("source")] public string Source { get; }

    [JsonPropertyName("publishTime")] public DateTime PublishTime { get; }

    [JsonPropertyName("data")] public EmailIntelligenceData Data { get; set; }

    public static EmailIntelligencePublisherMessage From(CustomerProfileEmailUpdatedRequest request)
    {
        return new EmailIntelligencePublisherMessage
        {
            Data = new EmailIntelligenceData
            {
                AccountNumber = request.AccountDetailNumber,
                AccountStatusId = request.AccountStatusId,
                CrmUserId = request.CrmUserId,
                EffectiveDate = request.EffectiveDate,
                Email = request.Email,
                EnterpriseProfileId = request.EnterpriseProfileId,
                FirstName = request.FirstName,
                LastName = request.LastName,
                PhoneNumber = request.PhoneNumber,
                ProfileId = request.ProfileId,
                City = request.City,
                Country = request.Country,
                State = request.State,
                Street = request.Street,
                ZipCode = request.ZipCode
            }
        };
    }
}
