using AutoMapper;
using Conso.Contracts.Internal;
using Conso.Providers.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Conso.Providers.Mapping
{
    public class ConsoProvidersProfile : Profile
    {
        public ConsoProvidersProfile()
        {
            CreateMap<ClassModel, ClassEntity>()
                .ForMember(x=> x.Id, opt=>opt.MapFrom(src => src.Id))
                .ForMember(x => x.Name, opt => opt.MapFrom(src => src.Name))
                .ReverseMap();
        }
    }
}
