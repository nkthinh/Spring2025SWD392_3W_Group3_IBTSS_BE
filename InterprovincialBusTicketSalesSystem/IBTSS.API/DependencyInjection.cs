
using IBTSS.Repository.Repositories.CustomerRepository;
using IBTSS.Repository.Repositories.GenericRepository;
using IBTSS.Repository.Repositories.UserRepository;
using IBTSS.Repository.UnitOfWork;
using IBTSS.Service.Services.CustomerService;
using IBTSS.Service.Services.JWT;
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
            services.AddScoped<JwtService>();

            // Đăng ký các Repository
            services.AddScoped<ICustomerRepository, CustomerRepository>();
            services.AddScoped<IUserRepository, UserRepository>();

            return services;
        }
    }
}
