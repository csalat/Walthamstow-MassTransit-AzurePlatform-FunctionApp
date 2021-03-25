using System;
using MassTransit.Metadata;
using Microsoft.Azure.Functions.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection;
using Serilog;

namespace Walthamstow.MassTransit.AzurePlatform.FunctionApp
{
    internal static class FunctionAppPlatformExtensions
    {
        internal static IFunctionsHostBuilder AddStartups(this IFunctionsHostBuilder builder, Type[] startupTypes)
        {
            foreach (var startupType in startupTypes)
            {
                Log.Information("Adding Startup: {StartupType}", TypeMetadataCache.GetShortName(startupType));
                builder.Services.AddSingleton(typeof(IFunctionAppPlatformStartup), startupType);
            }
            return builder;
        }
    }
}