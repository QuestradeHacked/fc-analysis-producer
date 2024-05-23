using FluentValidation;
using Google.Cloud.Storage.V1;
using Microsoft.Extensions.Logging;
using Polly;
using Questrade.FinCrime.Analysis.Producer.Infra.Config.CloudStorage;
using Object = Google.Apis.Storage.v1.Data.Object;

namespace Questrade.FinCrime.Analysis.Producer.Infra.CloudStorage;

public class AnalysisProducerCloudStorage : IAnalysisProducerCloudStorage
{
    private readonly ILogger<AnalysisProducerCloudStorage> _logger;
    private readonly Resilience _resilience;
    private readonly StorageClient _storageClient;
    private readonly IValidator<Object> _validator;
    
    public AnalysisProducerCloudStorage(
        ILogger<AnalysisProducerCloudStorage> logger,
        Resilience resilience,
        IValidator<Object> validator)
    {
        _logger = logger;
        _storageClient = StorageClient.Create();
        _resilience = resilience;
        _validator = validator;
    }

    public async Task<byte[]?> DownloadFileAsync(string fileName, string bucket)
    {
        var bucketObject = await GetFileFromBucketAsync(fileName, bucket);

        if (bucketObject is null)
        {
            _logger.LogError("File not found");

            return null;
        }
        
        var validationResult = await _validator.ValidateAsync(bucketObject);

        if (!validationResult.IsValid)
        {
            _logger.LogError("Invalid file received: {@ValidationError}", validationResult.Errors);

            return null;
        }

        try
        {
            using var stream = new MemoryStream();
            AsyncPolicy retryPolicy = Policy.Handle<Exception>()
                .WaitAndRetryAsync(
                    _resilience.RetryCount,
                    retryAttempt =>
                        TimeSpan.FromSeconds(Math.Pow(2, retryAttempt)));

            await retryPolicy.ExecuteAsync(async () =>
                await _storageClient.DownloadObjectAsync(bucket, fileName, stream));

            stream.Position = 0;

            return stream.ToArray();
        }
        catch (Exception ex)
        {
            _logger.LogError("File download failed: {@Exception}", ex);

            return null;
        }
    }

    private async Task<Object?> GetFileFromBucketAsync(string objectName, string bucket)
    {
        if (string.IsNullOrEmpty(objectName))
            return null;

        try
        {
            return await _storageClient.GetObjectAsync(bucket, objectName);
        }
        catch
        {
            _logger.LogError("Failed to get object info");

            return null;
        }
    }
}
