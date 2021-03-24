using System;
using MassTransit.Metadata;
using Microsoft.Azure.Functions.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection;
using Serilog;
using Walthamstow.MassTransit.AzurePlatform.FunctionApp.Interfaces;

namespace Walthamstow.MassTransit.AzurePlatform.FunctionApp
{
    public static class MassTransitExtensions
    {
        public static IFunctionsHostBuilder AddMassTransitStartup(
            this IFunctionsHostBuilder builder,
            params Type[] startupTypes)
        {

            foreach (var startupType in startupTypes)
            {
                Log.Information("Adding Startup: {StartupType}", TypeMetadataCache.GetShortName(startupType));
                builder.Services.AddSingleton(typeof (IFunctionAppPlatformStartup), startupType);
            }

            MassTransitStartup.ConfigureServices(builder);
            return builder;
        }
        
    }
}