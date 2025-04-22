using AutoMapper;
using IBTSS.Repository.Entities;
using IBTSS.Service.DTO.Request.Customer;
using IBTSS.Service.DTO.Request.Membership;
using IBTSS.Service.DTO.Request.Route;
using IBTSS.Service.DTO.Request.Ticket;
using IBTSS.Service.DTO.Request.Transaction;
using IBTSS.Service.DTO.Request.Trip;
using IBTSS.Service.DTO.Response.Book;
using IBTSS.Service.DTO.Response.Customer;
using IBTSS.Service.DTO.Response.Membership;
using IBTSS.Service.DTO.Response.Route;
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
            CreateMap<TripRequest, TripResponse>().ForMember(dest => dest.IsDelete, opt => opt.MapFrom(src => "false"));
            CreateMap<Trip, TripSearchDto>();
            CreateMap<Book, TicketResponse>();
            CreateMap<TicketRequest, Book>();
            CreateMap<TransactionRequest, Transaction>();
            CreateMap<Transaction, TransactionResponse>();
            CreateMap<Repository.Entities.Route, RouteResponse>();
            CreateMap<RouteRequest, Repository.Entities.Route>().ForMember(dest => dest.IsDelete, opt => opt.MapFrom(src => "false"));

            CreateMap<Book, BookResponse>()
    .ForMember(dest => dest.SeatIds, opt => opt.MapFrom(src => src.Tickets.Select(t => t.SeatId)))
    .ForMember(dest => dest.Tickets, opt => opt.MapFrom(src => src.Tickets));

            CreateMap<Ticket, TicketResponse>();

        }
    }
}

