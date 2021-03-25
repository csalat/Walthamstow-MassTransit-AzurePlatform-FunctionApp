using System;
using Microsoft.ApplicationInsights.DependencyCollector;
using Microsoft.Azure.Functions.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Serilog.Events;
using Serilog.Extensions.Hosting;
using Serilog.Extensions.Logging;
using Walthamstow.MassTransit.AzurePlatform.Configs;
using ILogger = Serilog.ILogger;

namespace Walthamstow.MassTransit.AzurePlatform.FunctionApp
{
    public static class MassTransitExtensions
    {
        public static IFunctionsHostBuilder AddMassTransitStartup(
            this IFunctionsHostBuilder builder,
             Type[] startupTypes,params string[] jsonFileNames)
        {
            return builder.ConfigureDefaults(jsonFileNames)
                .AddStartups(startupTypes)
                .ConfigureServices();
        }


    }
}