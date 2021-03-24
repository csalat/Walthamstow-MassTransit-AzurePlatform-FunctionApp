using Microsoft.ApplicationInsights.DependencyCollector;
using Microsoft.Azure.Functions.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Serilog;
using Walthamstow.MassTransit.AzurePlatform.Configs;

namespace Walthamstow.MassTransit.AzurePlatform.FunctionApp.Logging
{
    public static class ApplicationInsightsExtensions
    {
        public static IFunctionsHostBuilder AddApplicationInsights(this IFunctionsHostBuilder builder)
        {
            if (string.IsNullOrWhiteSpace(JsonConfigurationReader.Read().GetSection("ApplicationInsights")
                ?.GetValue<string>("InstrumentationKey")))
                return builder;

            Log.Information("Configuring Application Insights");
            builder.Services.AddApplicationInsightsTelemetry();
            builder.Services.ConfigureTelemetryModule<DependencyTrackingTelemetryModule>((module, o) => module.IncludeDiagnosticSourceActivities.Add("MassTransit"));
            
            return builder;
        }
    }
}