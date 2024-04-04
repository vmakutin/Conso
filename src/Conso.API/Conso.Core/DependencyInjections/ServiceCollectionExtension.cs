using Conso.Core.Interfaces;
using Microsoft.Extensions.DependencyInjection;

namespace Conso.Core.DependencyInjections
{
    public static class ServiceCollectionExtension
    {
        public static IServiceCollection AddCore(this IServiceCollection services) 
        { 
            services.AddScoped<IServiceClass, ServiceClass>();
            return services;
        }
    }
}
