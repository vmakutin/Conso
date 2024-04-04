using Conso.Contracts.Internal;
using Conso.Core.Interfaces;
using Conso.Providers.Interfaces;
using Microsoft.Extensions.Logging;

namespace Conso.Core
{
    public class ServiceClass : IServiceClass
    {
        private readonly ILogger<ServiceClass> _logger;
        private readonly IProviderClass  _providerClass;

        public ServiceClass(
            ILogger<ServiceClass> logger,
            IProviderClass providerClass
            )
        {
            _logger = logger;
            _providerClass = providerClass;
        }
        public IEnumerable<ClassModel> DoGet()
        {
            return _providerClass.Get();
        }
    }
}
