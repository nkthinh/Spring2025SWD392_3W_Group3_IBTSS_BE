
using IBTSS.Repository.Repositories.CustomerRepository;
using IBTSS.Repository.Repositories.GenericRepository;
using IBTSS.Repository.UnitOfWork;
using IBTSS.Service.Services.CustomerService;

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



            // Đăng ký các Repository
            services.AddScoped<ICustomerRepository, CustomerRepository>();


            return services;
        }
    }
}
