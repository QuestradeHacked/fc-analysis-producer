using MediatR;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Moq;
using Newtonsoft.Json;
using NSubstitute;
using Questrade.FinCrime.Analysis.Producer.Domain.Repositories.GraphQL;
using Questrade.FinCrime.Analysis.Producer.Infra.CloudStorage;
using Questrade.FinCrime.Analysis.Producer.Infra.Models.Messages;
using Questrade.FinCrime.Analysis.Producer.Infra.Subscriber;
using Questrade.FinCrime.Analysis.Producer.Tests.Integration.Fixture;
using Questrade.FinCrime.Analysis.Producer.Tests.Integration.Providers;
using Questrade.Library.PubSubClientHelper.Primitives;
using Xunit;

namespace Questrade.FinCrime.Analysis.Producer.Tests.Integration.Infra.Subscriber;

public class AnalysisProducerSubscriberHandleReceivedMessageAsyncTests : IAssemblyFixture<PubSubEmulatorProcessFixture>
{
    private readonly MockLogger<AnalysisProducerSubscriberHandleReceivedMessageAsyncTests> _logger;

    private readonly PubSubEmulatorProcessFixture _pubSubFixture;

    private readonly AnalysisProducerSubscriber _subscriber;

    private readonly int _subscriberTimeout;

    private readonly string _topicId;

    public AnalysisProducerSubscriberHandleReceivedMessageAsyncTests(PubSubEmulatorProcessFixture pubSubFixture)
    {
        var cloudStorage = Substitute.For<IAnalysisProducerCloudStorage>();
        var customerRepository = Substitute.For<ICustomerRepository>();

        _logger = new MockLogger<AnalysisProducerSubscriberHandleReceivedMessageAsyncTests>();
        _pubSubFixture = pubSubFixture;
        _subscriberTimeout = _pubSubFixture.SubscriberTimeout;
        _topicId = $"T_{Guid.NewGuid()}";

        var mediator = Substitute.For<IMediator>();

        var loggerFactory = new Mock<ILoggerFactory>();
        loggerFactory.Setup(x => x.CreateLogger(It.IsAny<string>())).Returns(_logger);

        var services = new ServiceCollection();

        services.AddSingleton<IDefaultJsonSerializerOptionsProvider, MyDefaultJsonSerializerOptionsProvider>();
        var serviceProvider = services.AddMemoryCache().BuildServiceProvider();
        var subscriptionId = $"{_topicId}.{Guid.NewGuid()}";
        var subscriberConfig = _pubSubFixture.CreateDefaultSubscriberConfig(subscriptionId);

        _subscriber = new AnalysisProducerSubscriber(
            loggerFactory.Object,
            serviceProvider,
            cloudStorage,
            customerRepository,
            subscriberConfig,
            mediator
        );

        _pubSubFixture.CreateTopic(_topicId);
        _pubSubFixture.CreateSubscription(_topicId, subscriptionId);
    }

    [Theory]
    [MemberData(nameof(GetPossibleInvalidMessages))]
    public async Task HandleReceivedMessageAsync_ShouldLogWarning_WhenIdOrNameEmptyOrNull(AnalysisProducerMessage analysisProducerMessage)
    {
        // Arrange
        var publisher = await _pubSubFixture.CreatePublisherAsync(_topicId);

        // Act
        await publisher.PublishAsync(JsonConvert.SerializeObject(analysisProducerMessage));
        await _subscriber.StartAsync(CancellationToken.None);
        await Task.Delay(_subscriberTimeout);
        await _subscriber.StopAsync(CancellationToken.None);

        var loggedMessages = _logger.GetAllMessages();

        // Assert
        //FluentValidation
        Assert.Contains("A message with empty id or name was received", loggedMessages);
    }

    public static IEnumerable<object[]> GetPossibleInvalidMessages()
    {
        var faker = new Bogus.Faker();

        yield return new object[]
        {
            new AnalysisProducerMessage
            {
                Id =  faker.Random.Guid().ToString(),
                Name = null
            }
        };
    }
}
