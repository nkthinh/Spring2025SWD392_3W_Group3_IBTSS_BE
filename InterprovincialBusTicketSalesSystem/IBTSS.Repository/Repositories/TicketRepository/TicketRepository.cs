using IBTSS.Repository.Entities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IBTSS.Repository.Repositories.TicketRepository
{
    public class TicketRepository : ITicketRepository
    {
        private readonly AppDbContext _context;

        public TicketRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<List<Ticket>> GetAllAsync()
        {
            return await _context.Tickets
                .Include(t => t.Trip)
                    .ThenInclude(trip => trip.Route) // Load Route trong Trip
                .Include(t => t.Trip)
                    .ThenInclude(trip => trip.Bus) // Load Bus trong Trip
                .Include(t => t.Book) // Load Book để lấy CreatedAt, CustomerId
                    .ThenInclude(book => book.Customer)
                .ToListAsync();
        }
        public async Task<Ticket?> GetByIdAsync(string id) =>
            await _context.Tickets.Include(t => t.Trip) // Nạp Trip
        .ThenInclude(trip => trip.Route) // Nạp Route cho Trip
        .Include(t => t.Trip.Bus) // Nạp Bus cho Trip
        .FirstOrDefaultAsync(t => t.TicketId == id);

        public async Task AddAsync(Ticket ticket)
        {
            await _context.Tickets.AddAsync(ticket);
        }

        public async Task AddRangeAsync(List<Ticket> tickets)
        {
            await _context.Tickets.AddRangeAsync(tickets);
        }

        public async Task UpdateAsync(Ticket ticket)
        {
            _context.Tickets.Update(ticket);
        }

        public async Task<bool> DeleteAsync(string id)
        {
            var ticket = await _context.Tickets.FindAsync(id);
            if (ticket == null) return false;
            ticket.IsDelete = true;
            return true;
        }
    }
}