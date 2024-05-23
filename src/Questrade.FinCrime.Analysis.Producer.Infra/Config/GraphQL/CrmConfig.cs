using System.Diagnostics.CodeAnalysis;

namespace Questrade.FinCrime.Analysis.Producer.Infra.Config.GraphQL;

[ExcludeFromCodeCoverage]
public class CrmConfig
{
    public string Endpoint { get; set; } = default!;

    public string Token { get; set; } = default!;
}
