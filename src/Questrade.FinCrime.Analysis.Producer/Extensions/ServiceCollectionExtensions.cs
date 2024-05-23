using System.Net.Http.Headers;
using GraphQL.Client.Abstractions;
using GraphQL.Client.Http;
using GraphQL.Client.Serializer.Newtonsoft;
using MediatR;
using Microsoft.FeatureManagement;
using Questrade.FinCrime.Analysis.Producer.Config;
using Questrade.FinCrime.Analysis.Producer.Domain.Constants;
using Questrade.FinCrime.Analysis.Producer.Domain.Models;
using Questrade.FinCrime.Analysis.Producer.Domain.Repositories.GraphQL;
using Questrade.FinCrime.Analysis.Producer.Infra.CloudStorage;
using Questrade.FinCrime.Analysis.Producer.Infra.Config.CloudStorage;
using Questrade.FinCrime.Analysis.Producer.Infra.Config.GraphQL;
using Questrade.FinCrime.Analysis.Producer.Infra.Config.PubSub;
using Questrade.FinCrime.Analysis.Producer.Infra.Models.Messages;
using Questrade.FinCrime.Analysis.Producer.Infra.Repositories.GraphQL;
using Questrade.FinCrime.Analysis.Producer.Infra.Subscriber;
using Questrade.Library.HealthCheck.AspNetCore.Extensions;
using Questrade.Library.PubSubClientHelper.Extensions;
using Questrade.Library.PubSubClientHelper.HealthCheck;
using Questrade.Library.PubSubClientHelper.Primitives;
using Questrade.Library.PubSubClientHelper.Publisher.Outbox;
using Serilog;

namespace Questrade.FinCrime.Analysis.Producer.Extensions;

public static class ServiceCollectionExtensions
{
    internal static WebApplicationBuilder RegisterServices(
        this WebApplicationBuilder builder,
        AnalysisProducerConfiguration analysisProducerConfiguration
    )
    {
        builder.AddQuestradeHealthCheck();

        builder.Host.UseSerilog((context, logConfiguration) =>
            logConfiguration.ReadFrom.Configuration(context.Configuration));
        builder.Services.AddCrmService(builder.Configuration);
        builder.Services.AddControllers();
        builder.Services.AddAppServices();
        builder.Services.AddSubscribers(analysisProducerConfiguration);
        builder.Services.AddPublisher(analysisProducerConfiguration);
        builder.Services.AddFeatureManagement();
        builder.Services.AddMediatR(
            AppDomain.CurrentDomain.Load("Questrade.FinCrime.Analysis.Producer"),
            AppDomain.CurrentDomain.Load("Questrade.FinCrime.Analysis.Producer.Application"),
            AppDomain.CurrentDomain.Load("Questrade.FinCrime.Analysis.Producer.Infra")
        );

        return builder;
    }

    private static IServiceCollection AddCrmService(this IServiceCollection services, IConfiguration configuration)
    {
        var crmConfig = configuration.GetSection("Crm").Get<CrmConfig>();

        services.AddSingleton<IGraphQLClient>(provider =>
        {
            ValidateToken(crmConfig.Token, provider.GetService<IHostEnvironment>()?.EnvironmentName);

            var gqlHttpClient = CreateGraphQlHttpClient(crmConfig);
            return gqlHttpClient;
        });

        return services;
    }

    private static IServiceCollection AddSubscribers(this IServiceCollection services,
        AnalysisProducerConfiguration configuration)
    {
        if (configuration.AnalysisProducerSubscriberConfiguration!.Enable)
            services.RegisterSubscriber<
                AnalysisProducerSubscriberConfiguration,
                AnalysisProducerMessage,
                AnalysisProducerSubscriber,
                SubscriberHealthCheck<
                    AnalysisProducerSubscriberConfiguration,
                    AnalysisProducerMessage
                >
            >(configuration.AnalysisProducerSubscriberConfiguration);

        return services;
    }

    private static IServiceCollection AddPublisher(this IServiceCollection services,
        AnalysisProducerConfiguration configuration)
    {
        if (configuration.EmailIntelligencePublisherConfiguration!.Enable)
            RegisterOutboxPublisherWithInMemoryOutbox<EmailIntelligencePublisherConfiguration,
                EmailIntelligencePublisherMessage>(services, configuration.EmailIntelligencePublisherConfiguration);

        return services;
    }

    private static void RegisterOutboxPublisherWithInMemoryOutbox<TConfiguration, TPublisher>(
        IServiceCollection services, TConfiguration configuration)
        where TPublisher : class, new()
        where TConfiguration : class, IPublisherConfiguration<TPublisher>
    {
        services.RegisterOutboxPublisherWithInMemoryOutbox<
            TConfiguration,
            TPublisher,
            OutboxPubsubPublisherService<TConfiguration, TPublisher>,
            PubsubPublisherBackgroundService<TConfiguration, TPublisher>,
            PublisherHealthCheck<TConfiguration, TPublisher>
        >(configuration);
    }

    private static IServiceCollection AddAppServices(this IServiceCollection services)
    {
        // Services
        services.AddTransient<IAnalysisProducerCloudStorage, AnalysisProducerCloudStorage>();
        services.AddTransient<ICustomerRepository, CustomerRepository>();
        services.AddSingleton<Resilience>();

        return services;
    }

    private static bool IsTokenValid(string token, string? environmentName)
    {
        var environmentsRequired = new[] { QtEnvironments.UAT, QtEnvironments.Production };

        return !environmentsRequired.Contains(environmentName) || !string.IsNullOrWhiteSpace(token);
    }

    private static void ValidateToken(string token, string? environmentName)
    {
        if (!IsTokenValid(token, environmentName))
        {
            throw new InvalidOperationException("Authorization Token is required to target non-development environment.");
        }
    }

    private static GraphQLHttpClient CreateGraphQlHttpClient(CrmConfig crmConfig)
    {
        var gqlHttpClientOptions = new GraphQLHttpClientOptions
        {
            EndPoint = new Uri(crmConfig.Endpoint)
        };

        var gqlHttpClient = new GraphQLHttpClient(gqlHttpClientOptions, new NewtonsoftJsonSerializer());

        if (!string.IsNullOrWhiteSpace(crmConfig.Token))
        {
            gqlHttpClient.HttpClient.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue("Bearer", crmConfig.Token);
        }

        return gqlHttpClient;
    }
}
