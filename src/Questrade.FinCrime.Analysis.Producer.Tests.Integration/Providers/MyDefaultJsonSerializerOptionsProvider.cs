using System.Text.Json;
using System.Text.Json.Serialization;
using Questrade.Library.PubSubClientHelper.Primitives;

namespace Questrade.FinCrime.Analysis.Producer.Tests.Integration.Providers;

internal class MyDefaultJsonSerializerOptionsProvider : IDefaultJsonSerializerOptionsProvider
{
    public virtual JsonSerializerOptions GetJsonSerializerOptions()
    {
        var settings = new JsonSerializerOptions
        {
            DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull
        };

        return settings;
    }
}
