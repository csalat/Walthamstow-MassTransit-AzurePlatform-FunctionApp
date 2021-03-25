using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using MassTransit;
using MassTransit.Definition;
using MassTransit.ExtensionsDependencyInjectionIntegration;
using Microsoft.Azure.Functions.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Options;
using Serilog;
using Walthamstow.MassTransit.AzurePlatform.Configs;

namespace Walthamstow.MassTransit.AzurePlatform.FunctionApp
{
    internal static class MassTransitStartup
    {
        internal static IFunctionsHostBuilder ConfigureServices(this IFunctionsHostBuilder builder)
        {
            var configuration = builder.Services.BuildServiceProvider().GetService<IConfiguration>();
            Log.Information("Configuring MassTransit Services for Function Apps");
            var services = builder.Services;

            services.Configure<PlatformOptions>(configuration.GetSection("Platform"));
            services.Configure<ServiceBusOptions>(configuration.GetSection("ASB"));

            var provider = services.BuildServiceProvider();
            services.ConfigureSagaDbs(configuration);
            
            var platformStartups = provider
                .GetService<IEnumerable<IFunctionAppPlatformStartup>>()?.ToList();

            services.TryAddSingleton(KebabCaseEndpointNameFormatter.Instance);
            services.AddMassTransit(cfg =>
            {
                if (platformStartups != null)
                    foreach (var platformStartup in platformStartups)
                        platformStartup.ConfigurePlatform(cfg, services, configuration);

                SetupAzureServiceBus(provider, cfg, platformStartups);
            });

            return builder;
        }

        private static void SetupAzureServiceBus(IServiceProvider provider, IServiceCollectionBusConfigurator cfg,
            List<IFunctionAppPlatformStartup> platformStartups)
        {
            if (!IsUsingAzureServiceBus(provider))
                return;

            cfg.UsingAzureServiceBus((context, configure) =>
            {
                var options = context.GetRequiredService<IOptions<ServiceBusOptions>>().Value;
                if (string.IsNullOrWhiteSpace(options.ConnectionString))
                    throw new ConfigurationException("The Azure Service Bus ConnectionString must not be empty.");
                
                configure.Host(options.ConnectionString);
                configure.UseHealthCheck(context);
                
                if (platformStartups != null)
                    foreach (var platformStartup in platformStartups)
                        platformStartup.ConfigureBus(configure, context);

                configure.ConfigureEndpoints(context);
            });
        }

        private static bool IsUsingAzureServiceBus(IServiceProvider provider)
        {
            var platformOptions = provider.GetRequiredService<IOptions<PlatformOptions>>().Value;
            var transport = platformOptions.Transport.ToLower(CultureInfo.InvariantCulture);
            return transport != PlatformOptions.AzureServiceBus &&
                   transport != PlatformOptions.ASB;
        }
    }
}