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
            transaction.PaymentStatus = "Đã Thanh Toán"; // ✅ Tiếng Việt cho status
            transaction.Amount = pendingBook.TotalPrice;
            pendingBook.Status = "Hoàn Thành"; // ✅ Cập nhật trạng thái đơn
            pendingBook.TransactionId = transaction.TransactionId;

            int ticketCount = 0;

            foreach (var ticket in pendingBook.Tickets)
            {
                ticket.Status = "Hoàn Thành"; // ✅ Tiếng Việt cho ticket status
                ticket.IsDelete = false;
                ticket.isCancelled = false;
                ticketCount++;
            }

            var customer = await _context.Customers
                .FirstOrDefaultAsync(c => c.CustomerId == transaction.CustomerId);

            if (customer != null && transaction.PaymentStatus == "Đã Thanh Toán")
            {
                customer.Score += ticketCount; // ✅ Cộng điểm

                var memberships = await _context.Memberships
                    .Where(m => !m.IsDelete)
                    .OrderByDescending(m => m.MinTicketsRequired)
                    .ToListAsync();

                var eligibleMembership = memberships
                    .FirstOrDefault(m => customer.Score >= m.MinTicketsRequired);

                if (eligibleMembership != null)
                {
                    // Tìm ra rank cao nhất trong hệ thống
                    var highestRank = memberships.FirstOrDefault(m => m.MinTicketsRequired == memberships.Max(x => x.MinTicketsRequired));

                    if (customer.MembershipId == null || customer.MembershipId != eligibleMembership.MembershipId)
                    {
                        // Nếu lên hạng mới
                        customer.MembershipId = eligibleMembership.MembershipId;

                        if (highestRank != null && eligibleMembership.MembershipId == highestRank.MembershipId)
                        {
                            customer.DiscountQuotaLeft = 100; // ✅ Hạng cao nhất -> Giảm giá vĩnh viễn
                        }
                        else
                        {
                            customer.DiscountQuotaLeft = 5; // ✅ Các hạng khác -> +5 vé
                        }
                    }
                    else
                    {
                        // Nếu vẫn cùng hạng
                        if (highestRank != null && customer.MembershipId != highestRank.MembershipId)
                        {
                            if (customer.DiscountQuotaLeft == null)
                                customer.DiscountQuotaLeft = 5;
                            else
                                customer.DiscountQuotaLeft += 5; // ✅ Các rank thấp -> cộng thêm 5 vé
                        }
                        // Nếu đang ở rank cao nhất, thì không làm gì cả
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

