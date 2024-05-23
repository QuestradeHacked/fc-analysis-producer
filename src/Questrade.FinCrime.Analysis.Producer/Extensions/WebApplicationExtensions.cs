namespace Questrade.FinCrime.Analysis.Producer.Extensions;

public static class WebApplicationExtensions
{
    internal static WebApplication Configure(this WebApplication app)
    {
        if (app.Environment.IsDevelopment()) app.UseDeveloperExceptionPage();

        app.UseHttpsRedirection()
            .UseRouting()
            .UseEndpoints(endpoints => endpoints.MapControllers());

        return app;
    }
}
