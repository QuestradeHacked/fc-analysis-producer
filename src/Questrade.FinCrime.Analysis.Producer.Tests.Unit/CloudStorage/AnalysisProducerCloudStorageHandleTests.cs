using FluentValidation;
using FluentValidation.Results;
using Google.Cloud.Storage.V1;
using Microsoft.Extensions.Logging;
using Moq;
using NSubstitute;
using Questrade.FinCrime.Analysis.Producer.Infra.CloudStorage;
using Questrade.FinCrime.Analysis.Producer.Infra.Config.CloudStorage;
using Xunit;
using Object = Google.Apis.Storage.v1.Data.Object;

namespace Questrade.FinCrime.Analysis.Producer.Tests.Unit.CloudStorage;

public class AnalysisProducerCloudStorageTests
{
    private readonly AnalysisProducerCloudStorage _service;
    private readonly Mock<StorageClient> _client;


    public AnalysisProducerCloudStorageTests()
    {
        var logger = Substitute.For<ILogger<AnalysisProducerCloudStorage>>();
        var resilience = Substitute.For<Resilience>();
        var validator = new Mock<IValidator<Object>>();
        validator.Setup(v =>
            v.ValidateAsync(
                It.IsAny<IValidationContext>(),
                It.IsAny<CancellationToken>()
            )).ReturnsAsync(new ValidationResult());

        _client = new Mock<StorageClient>();

        _service = new AnalysisProducerCloudStorage(logger, resilience, validator.Object);
    }


    [Fact]
    public async void DownloadFileAsync_ShouldReturnsFalse_WhenFileDoesNotExists()
    {
        // Arrange
        const string bucketName = "bucket-name";
        const string fileExists = "fileDoesNotExist";
        const string fileName = "file-name";

        var validObject = new Google.Apis.Storage.v1.Data.Object();

        _client.Setup(m => m.GetObjectAsync(bucketName, fileExists, null, It.IsAny<CancellationToken>())).ReturnsAsync(validObject);

        _client.Setup(m => m.DownloadObjectAsync(
                bucketName,
                fileExists,
                It.IsAny<MemoryStream>(),
                null,
                It.IsAny<CancellationToken>(),
                null))
            .Throws(new Exception());

        // Act
        var response = await _service.DownloadFileAsync(fileName, bucketName);

        // Assert
        Assert.Null(response);
    }
}

