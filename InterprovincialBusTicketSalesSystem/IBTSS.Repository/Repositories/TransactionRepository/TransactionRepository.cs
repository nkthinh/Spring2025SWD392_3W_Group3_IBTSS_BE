using IBTSS.Repository.Entities;
using Microsoft.EntityFrameworkCore;

namespace IBTSS.Repository.Repositories.TransactionRepository
{
    public class TransactionRepository : ITransactionRepository
    {
        private readonly AppDbContext _context;

        public TransactionRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<List<Transaction>> GetAllAsync()
        {
            return await _context.Transactions.ToListAsync();
        }

        public async Task<Transaction?> GetByIdAsync(string id)
        {
            return await _context.Transactions.FindAsync(id);
        }
        public async Task<List<Transaction>> GetByCustomerIdAsync(string customerId)
        {
            return await _context.Transactions
                .Where(t => t.CustomerId == customerId && !t.IsDeleted)
                .ToListAsync();
        }


        public async Task<Transaction> AddAsync(Transaction transaction)
        {
            transaction.TransactionId = Guid.NewGuid().ToString();
            transaction.CreatedAt = DateTime.UtcNow;
            transaction.PaymentStatus = "Paid";

            // Lấy tất cả ticket của customer chưa có transaction
            var ticketsToUpdate = await _context.Tickets
                .Where(t => t.CustomerId == transaction.CustomerId
                         && t.TransactionId == null
                         && !t.IsCancelled
                         && !t.IsDelete)
                .ToListAsync();

            transaction.Amount = ticketsToUpdate.Sum(t => t.Price);

            // Gán TransactionId cho từng ticket
            foreach (var ticket in ticketsToUpdate)
            {
                ticket.TransactionId = transaction.TransactionId;
            }

            _context.Transactions.Add(transaction);
            await _context.SaveChangesAsync();
            return transaction;
        }

        public async Task<Transaction?> UpdateAsync(string id, Transaction transaction)
        {
            var existing = await _context.Transactions.FindAsync(id);
            if (existing == null) return null;

            existing.CustomerId = transaction.CustomerId;
            existing.CreatedAt = transaction.CreatedAt;
            existing.PaymentStatus = transaction.PaymentStatus;
            existing.Amount = transaction.Amount;

            await _context.SaveChangesAsync();
            return existing;
        }

        public async Task<bool> DeleteAsync(string id)
        {
            var transaction = await _context.Transactions.FindAsync(id);
            if (transaction == null) return false;

            _context.Transactions.Remove(transaction);
            await _context.SaveChangesAsync();
            return true;
        }
    }
}
