using IBTSS.Repository.Entities;
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
using System;
using System.Threading.Tasks;

namespace IBTSS.Repository.UnitOfWork
{
    public class UnitOfWork : IUnitOfWork, IDisposable
    {
        private readonly AppDbContext _context;

        public UnitOfWork(
            AppDbContext context,
            ICustomerRepository customerRepository,
            IUserRepository userRepository,
            IBusRepository busRepository,
            ILocationRepository locationRepository,
            ILocationRouteRepository locationRouteRepository,
            IRouteRepository routeRepository,
            ISeatRepository seatRepository,
            ITicketRepository ticketRepository,
            ITransactionRepository transactionRepository,
            ITripRepository tripRepository,
            IMembershipRepository membershipRepository,
            IBookRepository bookRepository
        )
        {
            _context = context;
            Customers = customerRepository;
            Users = userRepository;
            Buses = busRepository;
            Locations = locationRepository;
            LocationRoutes = locationRouteRepository;
            Routes = routeRepository;
            Seats = seatRepository;
            Tickets = ticketRepository;
            Transactions = transactionRepository;
            Trips = tripRepository;
            Memberships = membershipRepository;
            Books = bookRepository;
        }

        public ICustomerRepository Customers { get; }
        public IUserRepository Users { get; }
        public IBusRepository Buses { get; }
        public ILocationRepository Locations { get; }
        public ILocationRouteRepository LocationRoutes { get; }
        public IRouteRepository Routes { get; }
        public ISeatRepository Seats { get; }
        public ITicketRepository Tickets { get; }
        public ITransactionRepository Transactions { get; }
        public ITripRepository Trips { get; }
        public IMembershipRepository Memberships { get; }
        public IBookRepository Books { get; }

        public async Task<int> CompleteAsync()
            => await _context.SaveChangesAsync();

        public void Dispose()
            => _context.Dispose();
    }
}
