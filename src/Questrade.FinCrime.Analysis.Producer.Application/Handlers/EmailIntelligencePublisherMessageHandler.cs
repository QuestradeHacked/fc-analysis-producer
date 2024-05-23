using MediatR;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.FeatureManagement;
using Questrade.FinCrime.Analysis.Producer.Domain.Configuration;
using Questrade.FinCrime.Analysis.Producer.Domain.Models;
using Questrade.Library.PubSubClientHelper.Primitives;
using Questrade.Library.PubSubClientHelper.Publisher;

namespace Questrade.FinCrime.Analysis.Producer.Application.Handlers;

public class EmailIntelligencePublisherMessageHandler : AsyncRequestHandler<CustomerProfileEmailUpdatedRequest>
{
    private readonly IFeatureManager _featureManager;

    private readonly ILogger<EmailIntelligencePublisherMessageHandler> _logger;

    private readonly IPublisherService<EmailIntelligencePublisherMessage> _publisherService;

    public EmailIntelligencePublisherMessageHandler(ILogger<EmailIntelligencePublisherMessageHandler> logger,
        IServiceProvider serviceProvider, IFeatureManager featureManager)
    {
        _featureManager = featureManager;
        _logger = logger;
        _publisherService = serviceProvider.GetRequiredService<IPublisherService<EmailIntelligencePublisherMessage>>();
    }

    protected override async Task Handle(CustomerProfileEmailUpdatedRequest request,
        CancellationToken cancellationToken)
    {
        if (!await _featureManager.IsEnabledAsync(FeatureFlags.PublishEmailIntelligenceEventsFeature))
        {
            _logFeatureFlagOffInformation(_logger, FeatureFlags.PublishEmailIntelligenceEventsFeature, null);
            return;
        }

        try
        {
            await _publisherService.PublishMessageAsync(
                EmailIntelligencePublisherMessage.From(request),
                new PublishingOptions { TrackingId = Guid.NewGuid().ToString() }
            );

            _logEmailPublishedInformation(_logger, request.CrmUserId, null);
        }
        catch (Exception error)
        {
            _logEmailIntelligenceError(_logger, request.CrmUserId, error);
        }
    }

    private readonly Action<ILogger, string?, Exception?> _logEmailIntelligenceError = LoggerMessage.Define<string?>(
        eventId: new EventId(2, nameof(EmailIntelligencePublisherMessageHandler)),
        formatString: "Failed on publish an e-mail from {CrmUserId}",
        logLevel: LogLevel.Warning
    );

    private readonly Action<ILogger, string?, Exception?> _logEmailPublishedInformation =
        LoggerMessage.Define<string?>(
            eventId: new EventId(1, nameof(EmailIntelligencePublisherMessageHandler)),
            formatString: "Published e-mail from {CrmUserId}",
            logLevel: LogLevel.Information
        );

    private readonly Action<ILogger, string?, Exception?> _logFeatureFlagOffInformation = LoggerMessage.Define<string?>(
        eventId: new EventId(3, nameof(EmailIntelligencePublisherMessageHandler)),
        formatString: "Feature flag {featureFlag} is off.",
        logLevel: LogLevel.Information
    );
}
