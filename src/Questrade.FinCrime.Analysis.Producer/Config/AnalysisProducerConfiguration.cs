using Questrade.FinCrime.Analysis.Producer.Infra.Config.PubSub;

namespace Questrade.FinCrime.Analysis.Producer.Config;

public class AnalysisProducerConfiguration
{
    public AnalysisProducerSubscriberConfiguration? AnalysisProducerSubscriberConfiguration { get; set; }

    public EmailIntelligencePublisherConfiguration? EmailIntelligencePublisherConfiguration { get; set; }

    internal void Validate()
    {
        if (AnalysisProducerSubscriberConfiguration == null)
            throw new InvalidOperationException("Analysis Producer subscriber configuration is not valid.");

        AnalysisProducerSubscriberConfiguration.Validate();

        if (EmailIntelligencePublisherConfiguration == null)
            throw new InvalidOperationException("Email Intelligence Publisher configuration is not valid.");

        EmailIntelligencePublisherConfiguration.Validate();
    }
}
