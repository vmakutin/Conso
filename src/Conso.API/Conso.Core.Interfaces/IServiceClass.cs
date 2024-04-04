using Conso.Contracts.Internal;

namespace Conso.Core.Interfaces
{
    public interface IServiceClass
    {
        IEnumerable<ClassModel> DoGet();
    }
}
