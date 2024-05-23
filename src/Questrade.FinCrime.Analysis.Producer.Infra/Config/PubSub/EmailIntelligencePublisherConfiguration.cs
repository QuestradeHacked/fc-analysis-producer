using Microsoft.Extensions.Logging;
using Questrade.FinCrime.Analysis.Producer.Domain.Models;
using Questrade.Library.PubSubClientHelper.Primitives;

namespace Questrade.FinCrime.Analysis.Producer.Infra.Config.PubSub;

public class EmailIntelligencePublisherConfiguration : BasePublisherConfiguration<EmailIntelligencePublisherMessage>
{
    public override Task HandleMessageLogAsync(ILogger logger, LogLevel logLevel,
        EmailIntelligencePublisherMessage message, string logMessage, CancellationToken cancellationToken = default)
    {
        logger.Log(logLevel, "{LogMessage} - Message contents: {@Message}", logMessage, message);

        return Task.CompletedTask;
    }

    public virtual void Validate()
    {
        if (!Enable) return;

        if (string.IsNullOrWhiteSpace(ProjectId))
            throw new InvalidOperationException(
                $"The configuration options for the {ProjectId} publisher are not valid");

        if (string.IsNullOrWhiteSpace(TopicId))
            throw new InvalidOperationException($"The configuration options for the {TopicId} publisher are not valid");

        if (UseEmulator && string.IsNullOrWhiteSpace(Endpoint))
            throw new InvalidOperationException(
                $"The emulator configuration options for {Endpoint} publisher are not valid");
    }
}
