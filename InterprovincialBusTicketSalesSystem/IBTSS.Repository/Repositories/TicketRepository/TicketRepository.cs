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
        public async Task<List<Book>> GetAllAsync() =>
            await _context.Tickets.Where(t => !t.IsDelete).ToListAsync();

        public async Task<Book?> GetByIdAsync(string id) =>
            await _context.Tickets.FirstOrDefaultAsync(t => t.TicketId == id && !t.IsDelete);

        public async Task<Book> AddAsync(Book ticket)
        {
            // kiểm tra TransactionId nếu được gán
            if (!string.IsNullOrEmpty(ticket.TransactionId))
            {
                var exists = await _context.Transactions
                    .AnyAsync(t => t.TransactionId == ticket.TransactionId);

                if (!exists)
                {
                    throw new Exception($"TransactionId {ticket.TransactionId} không tồn tại.");
                }
            }

            // Lấy TicketId lớn nhất đang có
            var lastTicket = await _context.Tickets
                .OrderByDescending(t => t.TicketId)
                .FirstOrDefaultAsync();

            int nextId = 1;
            if (lastTicket != null && int.TryParse(lastTicket.TicketId, out int currentId))
            {
                nextId = currentId + 1;
            }

            ticket.TicketId = nextId.ToString("D5");
            ticket.CreatedAt = DateTime.UtcNow;

            _context.Tickets.Add(ticket);
            await _context.SaveChangesAsync();
            return ticket;
        }


        public async Task<Book> UpdateAsync(Book ticket)
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
        public async Task<List<Book>> GetByCustomerIdAsync(string customerId)
        {
            return await _context.Tickets
                .Where(t => t.CustomerId == customerId)
                .ToListAsync();
        }
        public async Task<List<Book>> GetUnpaidTicketsByCustomerId(string customerId)
        {
            return await _context.Tickets
                .Where(t => t.CustomerId == customerId && string.IsNullOrEmpty(t.TransactionId) && !t.IsDelete)
                .ToListAsync();
        }
        //thay thế cho SaveChange khi cần gọi 2 lần
        public async Task UpdateRangeAsync(List<Book> tickets)
        {
            _context.Tickets.UpdateRange(tickets);
            // KHÔNG GỌI SaveChangesAsync() nếu đang dùng UnitOfWork
        }

    }

}
