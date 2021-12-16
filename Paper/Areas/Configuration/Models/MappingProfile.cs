using ASC.Models.Models;
using AutoMapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Paper.Areas.Configuration.Models
{
    public class MappingProfile:Profile
    {
        public MappingProfile()
        {
            CreateMap<MasterDataKeyy, MasterDataKeyViewModel>();
            CreateMap<MasterDataKeyViewModel, MasterDataKeyy>();
            CreateMap<MasterDataValuey, MasterDataValueViewModel>();
            CreateMap<MasterDataValueViewModel, MasterDataValuey>();
        }
    }
}
