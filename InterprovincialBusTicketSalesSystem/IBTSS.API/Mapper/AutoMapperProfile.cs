using AutoMapper;
using IBTSS.Repository.Entities;
using IBTSS.Service.DTO.Request.Customer;
using IBTSS.Service.DTO.Request.Membership;
using IBTSS.Service.DTO.Request.Ticket;
using IBTSS.Service.DTO.Request.Transaction;
using IBTSS.Service.DTO.Request.Trip;
using IBTSS.Service.DTO.Response.Customer;
using IBTSS.Service.DTO.Response.Membership;
using IBTSS.Service.DTO.Response.Ticket;
using IBTSS.Service.DTO.Response.Transaction;
using IBTSS.Service.DTO.Response.Trip;
using IBTSS.Service.DTO.Response.User;

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
            CreateMap<User, AddUserResponse>(); 
            CreateMap<User, LoginUserResponse>();
            CreateMap<Membership, MembershipRequest>();
            CreateMap<Membership, MembershipResponse>();
            CreateMap<TripRequest, TripResponse>();
            CreateMap<Trip, TripSearchDto>();
            CreateMap<Ticket, TicketResponse>();
            CreateMap<TicketRequest, Ticket>();
            CreateMap<TransactionRequest, Transaction>();
            CreateMap<Transaction, TransactionResponse>();
        }
    }
}

