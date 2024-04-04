using AutoMapper;
using Conso.API.Gates.Rest.Dto;
using Conso.Contracts.Internal;
using Conso.Providers.Entities;

namespace Conso.API.Mapping
{
    public class ConsoApiProfiler: Profile
    {
        public ConsoApiProfiler() 
        {
            CreateMap<ClassModel, ClassDto>()
                .ForMember(x => x.Id, opt => opt.MapFrom(src => src.Id))
                .ForMember(x => x.Name, opt => opt.MapFrom(src => src.Name))
                .ReverseMap();
        }
    }
}
