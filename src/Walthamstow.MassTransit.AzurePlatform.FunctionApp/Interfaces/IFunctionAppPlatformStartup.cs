using MassTransit;
using MassTransit.ExtensionsDependencyInjectionIntegration;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Walthamstow.MassTransit.AzurePlatform.FunctionApp.Interfaces
{
    public interface IFunctionAppPlatformStartup
    {
        public void ConfigurePlatform(IServiceCollectionBusConfigurator configurator, IServiceCollection services,
            IConfiguration configuration);
        
        public void ConfigureBus<TEndpointConfigurator>(IBusFactoryConfigurator<TEndpointConfigurator> configurator, IBusRegistrationContext context)
            where TEndpointConfigurator : IReceiveEndpointConfigurator;
    }
}