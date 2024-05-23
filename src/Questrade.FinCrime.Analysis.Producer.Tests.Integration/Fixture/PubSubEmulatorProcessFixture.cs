using Google.Cloud.PubSub.V1;
using Grpc.Core;
using Questrade.FinCrime.Analysis.Producer.Infra.Config.PubSub;
using Questrade.FinCrime.Analysis.Producer.Tests.Integration.Config;

namespace Questrade.FinCrime.Analysis.Producer.Tests.Integration.Fixture;

public class PubSubEmulatorProcessFixture
{
    private readonly AppSettings _appSettings = new();

    private readonly PublisherServiceApiClient _publisherServiceApiClient;

    private readonly SubscriberServiceApiClient _subscriberServiceApiClient;

    public string Endpoint { get; private set; }

    public string ProjectId { get; private set; }

    public int SubscriberTimeout { get; private set; }

    public PubSubEmulatorProcessFixture()
    {
        Endpoint = $"{_appSettings.GetProcessPubSubHost()}:{_appSettings.GetProcessPubSubPort()}";
        Environment.SetEnvironmentVariable("PUBSUB_EMULATOR_HOST", Endpoint);

        ProjectId = _appSettings.GetPubSubProjectId();
        SubscriberTimeout = _appSettings.GetPubSubSubscriberTimeout();

        _publisherServiceApiClient = CreatePublisherServiceApiClient();
        _subscriberServiceApiClient = CreateSubscriberServiceApiClient();
    }

    public Topic CreateTopic(string topicId)
    {
        var topicName = TopicName.FromProjectTopic(ProjectId, topicId);
        var topic = _publisherServiceApiClient.CreateTopic(topicName);

        Console.WriteLine($"Topic {topic.Name} created.");

        return topic;
    }

    public Subscription CreateSubscription(string topicId, string subscriptionId)
    {
        var topicName = TopicName.FromProjectTopic(ProjectId, topicId);
        var subscriptionName = SubscriptionName.FromProjectSubscription(ProjectId, subscriptionId);

        var subscription = _subscriberServiceApiClient.CreateSubscription(
            new Subscription
            {
                TopicAsTopicName = topicName,
                SubscriptionName = subscriptionName,
                EnableMessageOrdering = true
            }
        );

        Console.WriteLine($"Subscription {subscriptionId} created.");

        return subscription;
    }

    public async Task<PublisherClient> CreatePublisherAsync(string topicId)
    {
        var publisherClientBuilder = new PublisherClientBuilder
        {
            ApiSettings = PublisherServiceApiSettings.GetDefault(),
            ChannelCredentials = ChannelCredentials.Insecure,
            Endpoint = Endpoint,
            Settings = new PublisherClient.Settings
            {
                EnableMessageOrdering = true
            },
            TopicName = TopicName.FromProjectTopic(ProjectId, topicId)
        };

        var publisher = await publisherClientBuilder.BuildAsync();

        return publisher;
    }

    public AnalysisProducerSubscriberConfiguration CreateDefaultSubscriberConfig(string subscriptionId)
    {
        return new AnalysisProducerSubscriberConfiguration
        {
            AcknowledgeDeadline = TimeSpan.FromSeconds(30),
            AcknowledgeExtensionWindow = TimeSpan.FromSeconds(10),
            Enable = true,
            Endpoint = Endpoint,
            MaximumOutstandingByteCount = 1,
            MaximumOutstandingElementCount = 1,
            ProjectId = ProjectId,
            SubscriberClientCount = 1,
            SubscriptionId = subscriptionId,
            UseEmulator = true
        };
    }

    private PublisherServiceApiClient CreatePublisherServiceApiClient()
    {
        var publisher = new PublisherServiceApiClientBuilder
        {
            ChannelCredentials = ChannelCredentials.Insecure,
            Endpoint = Endpoint
        }.Build();

        return publisher;
    }

    private SubscriberServiceApiClient CreateSubscriberServiceApiClient()
    {
        var subscriber = new SubscriberServiceApiClientBuilder
        {
            ChannelCredentials = ChannelCredentials.Insecure,
            Endpoint = Endpoint
        }.Build();

        return subscriber;
    }
}
