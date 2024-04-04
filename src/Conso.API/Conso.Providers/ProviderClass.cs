using AutoMapper;
using Conso.Contracts.Internal;
using Conso.Providers.Entities;
using Conso.Providers.Interfaces;

namespace Conso.Providers
{
    public class ProviderClass : RepositoryBase<ClassEntity, ConsoDbContext>, IProviderClass
    {
        public ProviderClass(ConsoDbContext dbContext,
            IMapper mapper)
            : base(dbContext, mapper)
        {
        }

        public IEnumerable<ClassModel> Get()
        {
            var all = GetAll();

            return Mapper.Map<IEnumerable<ClassModel>>(all.ToList());
        }
    }
}
