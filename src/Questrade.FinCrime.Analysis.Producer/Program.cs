using Questrade.FinCrime.Analysis.Producer.Config;
using Questrade.FinCrime.Analysis.Producer.Extensions;

var builder = WebApplication.CreateBuilder(args);

var analysisProducerConfiguration = new AnalysisProducerConfiguration();
builder.Configuration.Bind("AnalysisProducer", analysisProducerConfiguration);
analysisProducerConfiguration.Validate();

builder.RegisterServices(analysisProducerConfiguration);

var app = builder.Build().Configure();

app.Run();
