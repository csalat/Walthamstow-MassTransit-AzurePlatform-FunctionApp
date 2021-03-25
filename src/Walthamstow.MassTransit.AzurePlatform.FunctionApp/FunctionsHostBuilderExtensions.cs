using System;
using Microsoft.ApplicationInsights.DependencyCollector;
using Microsoft.Azure.Functions.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Serilog;
using Serilog.Events;
using Serilog.Extensions.Hosting;
using Serilog.Extensions.Logging;
using Walthamstow.MassTransit.AzurePlatform.Configs;
using ILogger = Serilog.ILogger;

namespace Walthamstow.MassTransit.AzurePlatform.FunctionApp
{
    internal static class FunctionsHostBuilderExtensions 
    {
        internal static IFunctionsHostBuilder ConfigureDefaults(this IFunctionsHostBuilder builder,params string[] jsonFileNames) =>
            builder.AddConfigFiles(jsonFileNames).AddLogger().AddSerilog().AddApplicationInsights();
        
        private static IFunctionsHostBuilder AddConfigFiles(this IFunctionsHostBuilder builder,params string[] jsonFileNames)
        {
            var configuration = JsonConfigurationReader.Read(jsonFileNames);
            builder.Services.AddSingleton(configuration);
            return builder;
        }
        
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
        
        private static IFunctionsHostBuilder AddSerilog(this IFunctionsHostBuilder builder, ILogger logger = null,
            bool dispose = false,
            LoggerProviderCollection loggerProviderCollection = null)
        {
            if (builder == null)
                throw new ArgumentNullException(nameof(builder));
        
            var services = builder.Services;
            if (loggerProviderCollection == null)
            {
                services.AddSingleton(p => (ILoggerFactory) new SerilogLoggerFactory(logger, dispose));
                services.ConfigureServices(logger);
                return builder;
            }
        
            services.AddSingleton(p =>
            {
                var serilogLoggerFactory = new SerilogLoggerFactory(logger, dispose, loggerProviderCollection);
                foreach (var loggerProvider in p.GetServices<ILoggerProvider>())
                {
                    serilogLoggerFactory.AddProvider(loggerProvider);
                }
        
                return (ILoggerFactory) serilogLoggerFactory;
            });
        
            services.ConfigureServices(logger);
            return builder;
        }
        
        private static void ConfigureServices(this IServiceCollection collection, ILogger logger)
        {
            if (collection == null)
                throw new ArgumentNullException(nameof(collection));
        
            if (logger != null)
                collection.AddSingleton(logger);
        
            var implementationInstance = new DiagnosticContext(logger);
            collection.AddSingleton(implementationInstance);
            collection.AddSingleton((IDiagnosticContext) implementationInstance);
        }
        
        private static IFunctionsHostBuilder AddApplicationInsights(this IFunctionsHostBuilder builder)
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