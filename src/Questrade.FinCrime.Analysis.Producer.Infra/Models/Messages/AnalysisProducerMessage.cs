namespace Questrade.FinCrime.Analysis.Producer.Infra.Models.Messages;

public class AnalysisProducerMessage
{
    public string Bucket { get; set; } = default!;

    public string ContentType { get; set; } = default!;

    public string? Id { get; set; }

    public string? Name { get; set; }

    public string TimeCreated { get; set; } = default!;

    public string Size { get; set; } = default!;
}
