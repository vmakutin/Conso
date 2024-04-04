using Conso.Contracts.Internal;

namespace Conso.Providers.Interfaces
{
    public interface IProviderClass
    {
        IEnumerable<ClassModel> Get();
    }
}
