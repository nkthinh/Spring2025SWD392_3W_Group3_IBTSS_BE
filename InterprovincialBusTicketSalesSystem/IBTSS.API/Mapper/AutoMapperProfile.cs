using AutoMapper;
using IBTSS.Repository.Entities;
using IBTSS.Service.DTO.Request.Customer;
using IBTSS.Service.DTO.Response;

namespace IBTSS.API.Mapper
{
    public class AutoMapperProfile : Profile
    {
        public AutoMapperProfile()
        {
            CreateMap<Customer, CustomerResponse>();
            CreateMap<CustomerRequest, Customer>()
                .ForMember(dest => dest.Score, opt => opt.MapFrom(src => 0))
                .ForMember(dest => dest.MembershipId, opt => opt.MapFrom(src => "001"));
        }
    }
}

