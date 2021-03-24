using Microsoft.Azure.Functions.Extensions.DependencyInjection;
using Serilog;
using Serilog.Events;
using Walthamstow.MassTransit.AzurePlatform.FunctionApp.Logging;

namespace Walthamstow.MassTransit.AzurePlatform.FunctionApp
{
    public static class FunctionsHostBuilderExtensions 
    {
        public static IFunctionsHostBuilder ConfigureDefaults(this IFunctionsHostBuilder builder) =>
            builder.AddLogger().AddSerilog().AddApplicationInsights();
        
        private static IFunctionsHostBuilder AddLogger(this IFunctionsHostBuilder builder)
        {
            Log.Logger = new LoggerConfiguration()
                .MinimumLevel.Debug()
                .MinimumLevel.Override("Microsoft", LogEventLevel.Information)
                .Enrich.FromLogContext()
                .WriteTo.Console()
                .CreateLogger();

            return builder;
        }
    }
}