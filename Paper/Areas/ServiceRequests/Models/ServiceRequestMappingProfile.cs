using ASC.Models.Models;
using AutoMapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Paper.Areas.ServiceRequests.Models
{
    public class ServiceRequestMappingProfile:Profile
    {
        public ServiceRequestMappingProfile()
        {
            CreateMap<ServiceRequestt, NewServiceRequestViewModel>();
            CreateMap<NewServiceRequestViewModel, ServiceRequestt>();
            CreateMap<ServiceRequestt, UpdateServiceRequestViewModel>();
            CreateMap<UpdateServiceRequestViewModel, ServiceRequestt>();

        }
    }
}
