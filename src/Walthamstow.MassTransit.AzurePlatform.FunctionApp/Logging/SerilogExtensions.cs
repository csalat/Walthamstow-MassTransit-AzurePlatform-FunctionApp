using System;
using Microsoft.Azure.Functions.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Serilog;
using Serilog.Extensions.Hosting;
using Serilog.Extensions.Logging;
using ILogger = Serilog.ILogger;

namespace Walthamstow.MassTransit.AzurePlatform.FunctionApp.Logging
{
    public static class SerilogExtensions
    {
        public static IFunctionsHostBuilder AddSerilog(this IFunctionsHostBuilder builder, ILogger logger = null,
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
    }
}