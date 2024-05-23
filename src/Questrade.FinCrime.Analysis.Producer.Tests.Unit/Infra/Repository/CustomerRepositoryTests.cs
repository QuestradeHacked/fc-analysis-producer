using GraphQL;
using GraphQL.Client.Abstractions;
using Microsoft.Extensions.Logging;
using NSubstitute;
using Questrade.FinCrime.Analysis.Producer.Domain.Exceptions;
using Questrade.FinCrime.Analysis.Producer.Domain.Models.CRM;
using Questrade.FinCrime.Analysis.Producer.Infra.Repositories.GraphQL;
using Questrade.FinCrime.Analysis.Producer.Tests.Unit.Faker;
using Xunit;

namespace Questrade.FinCrime.Analysis.Producer.Tests.Unit.Infra.Repository;

public class CustomerRepositoryTests
{
    private readonly CustomerRepository _customerRepository;
    private readonly IGraphQLClient _graphQlClient;

    public CustomerRepositoryTests()
    {
        _graphQlClient = Substitute.For<IGraphQLClient>();
        var logger = Substitute.For<ILogger<CustomerRepository>>();
        _customerRepository = new CustomerRepository(_graphQlClient, logger);
    }

    [Fact]
    public async Task GetCustomerInfo_ShouldReturnAException_WhenDataUserIdIsInvalid()
    {
        // Arrange
        var dataUserId = string.Empty;

        // Assert
        await Assert.ThrowsAsync<ArgumentException>(() =>
            _customerRepository.GetCustomerInfoAsync(dataUserId, CancellationToken.None));
    }

    [Fact]
    public async Task GetCustomerInfo_ShouldReturnValidCrmResponse_WhenAllParametersAreValid()
    {
        // Arrange
        var dataUserId = new Random().Next().ToString();
        _graphQlClient
            .SendQueryAsync<CrmResponse>(Arg.Any<GraphQLRequest>(), Arg.Any<CancellationToken>())
            .Returns(GraphQLResponseFaker.GenerateValidResponseForPersonId());

        //Act
        var customerProfile = await _customerRepository.GetCustomerInfoAsync(dataUserId, CancellationToken.None);

        // Assert
        Assert.NotNull(customerProfile.Item1);
        Assert.NotNull(customerProfile.Item2);
    }

    [Fact]
    public async Task GetCustomerInfo_WhenPersonIdNotExists_ShouldThrowsException()
    {
        // Arrange
        var dataUserId = new Random().Next().ToString();
        _graphQlClient
            .SendQueryAsync<CrmResponse>(Arg.Any<GraphQLRequest>(), Arg.Any<CancellationToken>())
            .Returns(GraphQLResponseFaker.GenerateNonPersonId());

        //Act
        await Assert.ThrowsAsync<ArgumentException>(() =>
            _customerRepository.GetCustomerInfoAsync(dataUserId, CancellationToken.None));
    }

    [Fact]
    public async Task GetProfile_ShouldReturnAException_WhenCrmReturnError()
    {
        // Arrange
        var dataUserId = new Random().Next().ToString();
        _graphQlClient
            .SendQueryAsync<CrmResponse>(Arg.Any<GraphQLRequest>(), Arg.Any<CancellationToken>())
            .Returns(GraphQLResponseFaker.GenerateInvalidResponse());

        // Assert
        await Assert.ThrowsAsync<CrmException>(() =>
            _customerRepository.GetCustomerInfoAsync(dataUserId, CancellationToken.None));
    }
}
