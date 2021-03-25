using System;
using System.Reflection;
using AzureFunctions.Extensions.Swashbuckle;
using Microsoft.Azure.Functions.Extensions.DependencyInjection;

namespace Walthamstow.MassTransit.AzurePlatform.FunctionApp
{
    public static class MassTransitExtensions
    {
        public static IFunctionsHostBuilder AddMassTransitStartup(
            this IFunctionsHostBuilder builder,
             Type[] startupTypes, Assembly executingAssembly, params string[] jsonFileNames)
        {
            return builder.ConfigureDefaults(jsonFileNames)
                .AddStartups(startupTypes)
                .ConfigureServices()
                .AddSwashBuckle(executingAssembly);
        }


    }
}