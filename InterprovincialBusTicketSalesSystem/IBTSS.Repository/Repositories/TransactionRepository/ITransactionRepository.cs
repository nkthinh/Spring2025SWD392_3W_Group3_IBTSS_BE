using IBTSS.Repository.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace IBTSS.Repository.Repositories.TransactionRepository
{
    public interface ITransactionRepository
    {
        Task<List<Transaction>> GetAllAsync();
        Task<Transaction?> GetByIdAsync(string id);
        Task<List<Transaction>> GetByCustomerIdAsync(string customerId);
        Task<Transaction> AddAsync(Transaction transaction);
        Task<Transaction?> UpdateAsync(string id, Transaction transaction);
        Task<bool> DeleteAsync(string id);
    }
}
