namespace Questrade.FinCrime.Analysis.Producer.Infra.CloudStorage;

public interface IAnalysisProducerCloudStorage
{
    Task<byte[]?> DownloadFileAsync(string filePath, string folderIfInfected);
}
