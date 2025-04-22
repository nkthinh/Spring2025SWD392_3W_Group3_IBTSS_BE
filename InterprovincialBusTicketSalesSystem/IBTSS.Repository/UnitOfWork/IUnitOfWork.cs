using IBTSS.Repository.Repositories.BookRepository;
using IBTSS.Repository.Repositories.BusRepository;
using IBTSS.Repository.Repositories.CustomerRepository;
using IBTSS.Repository.Repositories.LocationRepository;
using IBTSS.Repository.Repositories.LocationRouteRepository;
using IBTSS.Repository.Repositories.MembershipRepository;
using IBTSS.Repository.Repositories.RouteRepository;
using IBTSS.Repository.Repositories.SeatRepository;
using IBTSS.Repository.Repositories.TicketRepository;
using IBTSS.Repository.Repositories.TransactionRepository;
using IBTSS.Repository.Repositories.TripRepository;
using IBTSS.Repository.Repositories.UserRepository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IBTSS.Repository.UnitOfWork
{
    public interface IUnitOfWork : IDisposable
    {
        ICustomerRepository Customers { get; }
        IUserRepository Users { get; }
        IBusRepository Buses { get; }
        ILocationRepository Locations { get; }
        ILocationRouteRepository LocationRoutes { get; }
        IRouteRepository Routes { get; }
        ISeatRepository Seats { get; }
        ITicketRepository Tickets { get; }
        ITransactionRepository Transactions { get; }
        ITripRepository Trips { get; }
        IMembershipRepository Memberships { get; }
        IBookRepository Books { get; }
        Task<int> CompleteAsync();
    }
}
