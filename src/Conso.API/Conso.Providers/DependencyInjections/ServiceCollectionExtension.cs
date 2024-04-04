using Conso.Providers;
using Conso.Providers.EventHub;
using Conso.Providers.Interfaces;
using Conso.Providers.Interfaces.EventHub;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Conso.Providers.DependencyInjections
{
    public static class ServiceCollectionExtension
    {
        public static IServiceCollection AddProviders(this IServiceCollection services, IConfiguration config) 
        {
            services.AddScoped<IProviderClass, ProviderClass>();

            services.Configure<EventHubConfig>(config.GetSection("Conso:EventHub"));
            services.AddScoped<IEventHubProvider, EventHubProvider>();

            return services;
        }
    }
}
