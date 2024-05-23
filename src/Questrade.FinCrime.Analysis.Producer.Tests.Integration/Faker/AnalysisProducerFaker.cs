using Questrade.FinCrime.Analysis.Producer.Infra.Models.Messages;

namespace Questrade.FinCrime.Analysis.Producer.Tests.Integration.Faker;

public static class AnalysisProducerFaker
{
    public static AnalysisProducerMessage GetAnalysisProducerMessageFake()
    {
        var faker = new Bogus.Faker();

        var generatedAnalysisProducerMessage = new AnalysisProducerMessage
        {
            Bucket = faker.Random.Guid().ToString(),
            ContentType = faker.Random.Guid().ToString(),
            Id = faker.Random.Number().ToString(),
            Name = faker.Random.Guid().ToString(),
            TimeCreated = faker.Random.Guid().ToString(),
            Size = faker.Random.Guid().ToString(),
        };

        return generatedAnalysisProducerMessage;
    }
}
