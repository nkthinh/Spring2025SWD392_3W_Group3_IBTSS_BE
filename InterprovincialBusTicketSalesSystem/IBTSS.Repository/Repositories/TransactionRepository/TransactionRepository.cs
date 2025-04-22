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
            // Bước 1: Tìm Book có trạng thái Pending của Customer
            var pendingBook = await _context.Books
                .Include(b => b.Tickets)
                .FirstOrDefaultAsync(b =>
                    b.CustomerId == transaction.CustomerId &&
                    b.Status == "Pending");

            if (pendingBook == null)
            {
                // Không có đơn đặt nào ở trạng thái Pending => không tạo transaction
                return null; // hoặc throw new Exception("No pending booking found.");
            }

            transaction.TransactionId = Guid.NewGuid().ToString();
            transaction.CreatedAt = DateTime.UtcNow;
            transaction.PaymentStatus = "Paid";
            transaction.Amount = pendingBook.TotalPrice;
            pendingBook.Status = "Complete";

            pendingBook.TransactionId = transaction.TransactionId;

            // ✅ Cập nhật trạng thái của từng Ticket trong Book
            foreach (var ticket in pendingBook.Tickets)
            {
                ticket.Status = "Complete";          // Hoặc "Confirmed"
                ticket.IsDelete = false;         // Optional: đảm bảo chưa bị xóa
                ticket.isCancelled = false;      // Optional: chưa bị hủy
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

