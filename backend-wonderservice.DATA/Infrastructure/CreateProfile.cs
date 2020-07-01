using System;
using System.Collections.Generic;
using System.Text;
using AutoMapper;
using backend_wonderservice.DATA.Extensions;
using backend_wonderservice.DATA.Models;
using WonderService.Data.ViewModel;

namespace backend_wonderservice.DATA.Infrastructure
{
    public class CreateProfile : Profile
    {
        public CreateProfile()
        {

            CreateMap<UserModel, User>().ReverseMap();
            CreateMap<LoginModel, User>();
            CreateMap<UserViewModel, User>().ReverseMap();
            CreateMap<Customer, OrderModel>().ForMember(o => o.ServiceType, p => p.MapFrom(o => o.ServicesType.ServiceType)).ForMember(o => o.LocalGovernment, s => s.MapFrom(o => o.LocalGovernment.Name)).ForMember(o => o.States, s => s.MapFrom(d => d.State.Name));
            CreateMap<OrderModel, Customer>().ForMember(o => o.LocalGovernmentId, p => p.Ignore())
                .ForMember(o => o.ServicesType, s => s.Ignore()).ForMember(r => r.LocalGovernment, l => l.Ignore())
                .ForMember(o => o.State, l => l.Ignore()).ForMember(s => s.StateId, d => d.Ignore());
            CreateMap<ServicesTypes, ServiceTypeVm>().ReverseMap();

            CreateMap<PhotoVm, Photo>().ReverseMap();
            CreateMap<ServiceReturnVm, Services>().ForMember(o => o.Photo, p => p.MapFrom(s => s.Photos)).AfterMap((
                    src, des) =>
                {
                    des.Entry = DateTime.Now;
                })
                .ReverseMap();
            CreateMap<States, StateLocalGovernmentVm>().ForMember(r => r.Value, o => o.MapFrom(s => s.Name))
                .ReverseMap();
            CreateMap<LocalGovernment, StateLocalGovernmentVm>().ForMember(e => e.Value, s => s.MapFrom(o => o.Name))
                .ReverseMap();
            CreateMap<ServicesTypes, StateLocalGovernmentVm>().ForMember(e => e.Value, s => s.MapFrom(o => o.ServiceType))
                .ReverseMap();

        }
    }
}
