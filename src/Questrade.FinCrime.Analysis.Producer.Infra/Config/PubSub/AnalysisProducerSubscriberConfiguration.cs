using Microsoft.Extensions.Logging;
using Questrade.FinCrime.Analysis.Producer.Infra.Models.Messages;
using Questrade.Library.PubSubClientHelper.Primitives;

namespace Questrade.FinCrime.Analysis.Producer.Infra.Config.PubSub;

public class AnalysisProducerSubscriberConfiguration : BaseSubscriberConfiguration<AnalysisProducerMessage>
{
    public override Task HandleMessageLogAsync(ILogger logger, LogLevel logLevel, AnalysisProducerMessage message,
        string logMessage, Exception? error = null, CancellationToken cancellationToken = default)
    {
        logger.Log(logLevel, error, "{LogMessage} - Message with: {MessageId}", logMessage, message.Id);

        return Task.CompletedTask;
    }

    public void Validate()
    {
        if (!Enable) return;

        if (string.IsNullOrWhiteSpace(ProjectId) || string.IsNullOrWhiteSpace(SubscriptionId))
            throw new InvalidOperationException(
                "The configuration options for the AnalysisProducerSubscriber is not valid");

        if (UseEmulator && string.IsNullOrWhiteSpace(Endpoint))
            throw new InvalidOperationException(
                "The emulator configuration options for AnalysisProducerSubscriber is not valid");
    }
}
