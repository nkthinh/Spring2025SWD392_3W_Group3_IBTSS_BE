
using IBTSS.Repository.Repositories.BookRepository;
using IBTSS.Repository.Repositories.BusRepository;
using IBTSS.Repository.Repositories.CustomerRepository;
using IBTSS.Repository.Repositories.GenericRepository;
using IBTSS.Repository.Repositories.LocationRepository;
using IBTSS.Repository.Repositories.LocationRouteRepository;
using IBTSS.Repository.Repositories.MembershipRepository;
using IBTSS.Repository.Repositories.RouteRepository;
using IBTSS.Repository.Repositories.SeatRepository;
using IBTSS.Repository.Repositories.TicketRepository;
using IBTSS.Repository.Repositories.TransactionRepository;
using IBTSS.Repository.Repositories.TripRepository;
using IBTSS.Repository.Repositories.UserRepository;
using IBTSS.Repository.UnitOfWork;
using IBTSS.Service.Services.BookService;
using IBTSS.Service.Services.BusService;
using IBTSS.Service.Services.CustomerService;
using IBTSS.Service.Services.JWT;
using IBTSS.Service.Services.LocationService;
using IBTSS.Service.Services.MembershipService;
using IBTSS.Service.Services.RouteService;
using IBTSS.Service.Services.SeatService;
using IBTSS.Service.Services.TicketService;
using IBTSS.Service.Services.TransactionService;
using IBTSS.Service.Services.TripService;
using IBTSS.Service.Services.UserService;

namespace IBTSS.API
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddApplicationServices(this IServiceCollection services)
        {
            // Đăng ký các repository chung
            services.AddScoped(typeof(IGenericRepository<>), typeof(GenericRepository<>));

            // Đăng ký UnitOfWork
            services.AddScoped<IUnitOfWork, UnitOfWork>();

            // Đăng ký các Service
            services.AddScoped<ICustomerService, CustomerService>();
            services.AddScoped<IUserService, UserService>();
            services.AddScoped<IMembershipService, MembershipService>();
            services.AddScoped<ITripService, TripService>();
            services.AddScoped<ISeatService, SeatService>();
            services.AddScoped<IBusService, BusService>();
            services.AddScoped<JwtService>();
            services.AddScoped<ITicketService, TicketService>();
            services.AddScoped<ITransactionService, TransactionService>();
            services.AddScoped<ILocationService, LocationService>();
            services.AddScoped<IRouteService, RouteService>();
            services.AddScoped<IBookService, BookService>();


            // Đăng ký các Repository
            services.AddScoped<ICustomerRepository, CustomerRepository>();
            services.AddScoped<IUserRepository, UserRepository>();
            services.AddScoped<IMembershipRepository, MembershipRepository>();
            services.AddScoped<IBusRepository, BusRepository>();
            services.AddScoped<ILocationRepository, LocationRepository>();
            services.AddScoped<ILocationRouteRepository, LocationRouteRepository>();
            services.AddScoped<IRouteRepository, RouteRepository>();
            services.AddScoped<ISeatRepository, SeatRepository>();
            services.AddScoped<ITicketRepository, TicketRepository>();
            services.AddScoped<ITransactionRepository, TransactionRepository>();
            services.AddScoped<ITripRepository, TripRepository>();
            services.AddScoped<IBookRepository, BookRepository>();
            return services;
        }
    }
}
