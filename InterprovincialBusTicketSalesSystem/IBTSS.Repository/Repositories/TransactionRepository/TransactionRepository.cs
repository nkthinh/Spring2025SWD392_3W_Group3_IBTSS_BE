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
            var pendingBook = await _context.Books
                .Include(b => b.Tickets)
                .ThenInclude(b => b.Seat)
                .FirstOrDefaultAsync(b =>
                    b.CustomerId == transaction.CustomerId &&
                    b.Status == "Đang xử lý");

            if (pendingBook == null)
            {
                return null;
            }

            transaction.TransactionId = Guid.NewGuid().ToString();
            transaction.CreatedAt = DateTime.UtcNow;
            transaction.PaymentStatus = "Đã Thanh Toán";
            transaction.Amount = pendingBook.TotalPrice;
            pendingBook.Status = "Hoàn Thành";
            pendingBook.TransactionId = transaction.TransactionId;

            int ticketCount = 0;

            // ✅ Cập nhật trạng thái của từng Ticket
            foreach (var ticket in pendingBook.Tickets)
            {
                ticket.Status = "Hoàn Thành";
                ticket.IsDelete = false;
                ticket.isCancelled = false;
                ticketCount++; // ✅ Đếm số vé đã xử lý

                //if (ticket.Seat != null)
                //{
                //    ticket.Seat.IsBooked = true;
                //}
            }

            // ✅ Cập nhật điểm + membership cho customer
            var customer = await _context.Customers
                .FirstOrDefaultAsync(c => c.CustomerId == transaction.CustomerId);

            if (customer != null && transaction.PaymentStatus == "Đã Thanh Toán")
            {
                customer.Score += ticketCount;

                //// ✅ Cập nhật Membership nếu đủ điều kiện
                //var memberships = await _context.Memberships
                //    .Where(m => !m.IsDelete)
                //    .OrderByDescending(m => m.MinTicketsRequired)
                //    .ToListAsync();

                //foreach (var member in memberships)
                //{
                //    if (customer.Score >= member.MinTicketsRequired)
                //    {
                //        customer.MembershipId = member.MembershipId;
                //        break;
                //    }
                //}

                var memberships = await _context.Memberships
                    .Where(m => !m.IsDelete)
                    .OrderByDescending(m => m.MinTicketsRequired)
                    .ToListAsync();

                var eligibleMembership = memberships
                    .FirstOrDefault(m => customer.Score >= m.MinTicketsRequired);

                if (eligibleMembership != null)
                {
                    if (customer.MembershipId == null || customer.MembershipId != eligibleMembership.MembershipId)
                    {
                        customer.MembershipId = eligibleMembership.MembershipId;
                        customer.DiscountQuotaLeft = 5;
                    }
                    else if (customer.DiscountQuotaLeft == null)
                    {
                        customer.DiscountQuotaLeft = 5;
                    }
                }
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

