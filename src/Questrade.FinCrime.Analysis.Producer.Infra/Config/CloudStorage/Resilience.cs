using System.Diagnostics.CodeAnalysis;

namespace Questrade.FinCrime.Analysis.Producer.Infra.Config.CloudStorage;

[ExcludeFromCodeCoverage]
public class Resilience
{
    public int RetryCount { get; set; } = 3;
}
