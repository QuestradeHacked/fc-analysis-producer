using System.Globalization;
using CsvHelper;
using CsvHelper.Configuration;
using MediatR;
using Microsoft.Extensions.Logging;
using Questrade.FinCrime.Analysis.Producer.Domain.Models;
using Questrade.FinCrime.Analysis.Producer.Domain.Models.CRM;
using Questrade.FinCrime.Analysis.Producer.Domain.Repositories.GraphQL;
using Questrade.FinCrime.Analysis.Producer.Infra.CloudStorage;
using Questrade.FinCrime.Analysis.Producer.Infra.Config.PubSub;
using Questrade.FinCrime.Analysis.Producer.Infra.Models.Messages;
using Questrade.Library.PubSubClientHelper.Subscriber;

namespace Questrade.FinCrime.Analysis.Producer.Infra.Subscriber;

public class AnalysisProducerSubscriber : PubsubSubscriberBackgroundService<AnalysisProducerSubscriberConfiguration,
    AnalysisProducerMessage>
{
    private readonly IAnalysisProducerCloudStorage _cloudStorage;
    private readonly ICustomerRepository _customerRepository;
    private readonly IMediator _mediator;

    public AnalysisProducerSubscriber(
        ILoggerFactory loggerFactory,
        IServiceProvider serviceProvider,
        IAnalysisProducerCloudStorage cloudStorage,
        ICustomerRepository customerRepository,
        AnalysisProducerSubscriberConfiguration subscriberConfiguration,
        IMediator mediator)
        : base(loggerFactory, subscriberConfiguration, serviceProvider)
    {
        _cloudStorage = cloudStorage;
        _customerRepository = customerRepository;
        _mediator = mediator;
    }

    protected override async Task<bool> HandleReceivedMessageAsync(AnalysisProducerMessage message,
        CancellationToken cancellationToken)
    {
        _logDefineScope(Logger, nameof(AnalysisProducerSubscriber), nameof(HandleReceivedMessageAsync));
        _logMessageReceivedInformation(Logger, message, null);

        if (string.IsNullOrEmpty(message.Id) || string.IsNullOrEmpty(message.Name))
        {
            _logMissingRequiredInformationWarning(Logger, null);

            return true;
        }

        try
        {
            var fileByte = await _cloudStorage.DownloadFileAsync(message.Name, message.Bucket);

            if (fileByte != null)
            {
                IList<CsvRecord> records;

                using var reader = new MemoryStream(fileByte);
                reader.Position = 0;
                using (var csv = new CsvReader(new StreamReader(reader),
                           new CsvConfiguration(CultureInfo.InvariantCulture)))
                {
                    records = csv.GetRecords<CsvRecord>().ToList();
                }

                var userIds = records.Select(record => record.UserID)
                    .Where(userId => !string.IsNullOrEmpty(userId) && userId.All(char.IsDigit))
                    .ToList();

                _logMessageUserIdsFoundDebug(Logger, userIds.Count, null);

                foreach (var userId in userIds)
                {
                    var (person, personAccount) =
                        await _customerRepository.GetCustomerInfoAsync(userId!, cancellationToken);

                    if (person == null || personAccount == null)
                        continue;

                    var requestOrchestrator =
                        new CustomerProfileEmailUpdatedRequest(userId!, person, personAccount);
                    await _mediator.Send(requestOrchestrator, cancellationToken);
                }
            }

            else
            {
                _logHandlingMessageError(Logger, nameof(AnalysisProducerSubscriber), message.Id, null);
            }

            return true;
        }
        catch (Exception error)
        {
            _logHandlingMessageError(Logger, nameof(AnalysisProducerSubscriber), message.Id, error);
            return false;
        }
    }

    private readonly Func<ILogger, string?, string?, IDisposable> _logDefineScope =
        LoggerMessage.DefineScope<string?, string?>(
            "{AnalysisProducerSubscriberName}:{HandleReceivedMessageAsyncName}"
        );

    private readonly Action<ILogger, string?, string?, Exception?> _logHandlingMessageError =
        LoggerMessage.Define<string?, string?>(
            eventId: new EventId(3, nameof(AnalysisProducerSubscriber)),
            formatString: "Failed on handling the received message from {AnalysisProducerSubscriberName}: {MessageId}",
            logLevel: LogLevel.Error
        );

    private readonly Action<ILogger, AnalysisProducerMessage?, Exception?> _logMessageReceivedInformation =
        LoggerMessage.Define<AnalysisProducerMessage?>(
            eventId: new EventId(1, nameof(AnalysisProducerSubscriber)),
            formatString: "Message received: @{Message}",
            logLevel: LogLevel.Information
        );

    private readonly Action<ILogger, Exception?> _logMissingRequiredInformationWarning =
        LoggerMessage.Define(
            eventId: new EventId(2, nameof(AnalysisProducerSubscriber)),
            formatString: "A message with empty id or name was received",
            logLevel: LogLevel.Warning
        );

    private readonly Action<ILogger, int?, Exception?> _logMessageUserIdsFoundDebug =
        LoggerMessage.Define<int?>(
            eventId: new EventId(4, nameof(AnalysisProducerSubscriber)),
            formatString: "Total UserId found: {totalUserId}",
            logLevel: LogLevel.Debug
        );
}
