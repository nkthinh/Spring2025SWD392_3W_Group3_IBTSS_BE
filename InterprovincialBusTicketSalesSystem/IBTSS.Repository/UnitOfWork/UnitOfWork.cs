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
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IBTSS.Repository.UnitOfWork
{
    public class UnitOfWork(AppDbContext _context, ICustomerRepository _customerRepository, IUserRepository _userRepository, IBusRepository _busRepository
        , ILocationRepository _locationRepository, ILocationRouteRepository _locationRouteRepository, IRouteRepository _routeRepository
        , ISeatRepository _seatRepository, ITicketRepository _ticketRepository, ITransactionRepository _transactionRepository, ITripRepository _tripRepository
        , IMembershipRepository membershipRepository) : IUnitOfWork
    {
        public ICustomerRepository Customers { get; } = _customerRepository;
        public IUserRepository Users { get; } = _userRepository;
        public IBusRepository Buses { get; } = _busRepository;
        public ILocationRepository Locations { get; } = _locationRepository;
        public ILocationRouteRepository LocationRoutes { get; } = _locationRouteRepository;
        public IRouteRepository Routes { get; } = _routeRepository;
        public ISeatRepository Seats { get; } = _seatRepository;
        public ITicketRepository Tickets { get; } = _ticketRepository;
        public ITransactionRepository Transactions { get; } = _transactionRepository;
        public ITripRepository Trips { get; } = _tripRepository;
        public IMembershipRepository Memberships { get; } = membershipRepository;

        public async Task<int> CompleteAsync()
        {
            return await _context.SaveChangesAsync();
        }

        public void Dispose()
        {
            _context.Dispose();
        }
    }
}
