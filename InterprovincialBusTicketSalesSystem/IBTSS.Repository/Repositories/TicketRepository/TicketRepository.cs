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
        public async Task<List<Ticket>> GetAllAsync() =>
            await _context.Tickets.Where(t => !t.IsDelete).ToListAsync();

        public async Task<Ticket?> GetByIdAsync(string id) =>
            await _context.Tickets.FirstOrDefaultAsync(t => t.TicketId == id && !t.IsDelete);

        public async Task<Ticket> AddAsync(Ticket ticket)
        {
            // Lấy TicketId lớn nhất đang có
            var lastTicket = await _context.Tickets
                .OrderByDescending(t => t.TicketId)
                .FirstOrDefaultAsync();

            int nextId = 1;

            if (lastTicket != null && int.TryParse(lastTicket.TicketId, out int currentId))
            {
                nextId = currentId + 1;
            }

            ticket.TicketId = nextId.ToString("D5"); // ví dụ: "00001"
            ticket.CreatedAt = DateTime.UtcNow;

            _context.Tickets.Add(ticket);
            await _context.SaveChangesAsync();
            return ticket;
        }

        public async Task<Ticket> UpdateAsync(Ticket ticket)
        {
            _context.Tickets.Update(ticket);
            await _context.SaveChangesAsync();
            return ticket;
        }

        public async Task<bool> DeleteAsync(string id)
        {
            var ticket = await _context.Tickets.FindAsync(id);
            if (ticket == null) return false;
            ticket.IsDelete = true;
            await _context.SaveChangesAsync();
            return true;
        }
        public async Task<List<Ticket>> GetByCustomerIdAsync(string customerId)
        {
            return await _context.Tickets
                .Where(t => t.CustomerId == customerId)
                .ToListAsync();
        }

    }

}
