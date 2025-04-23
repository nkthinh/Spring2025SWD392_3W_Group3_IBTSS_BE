using IBTSS.Repository.Entities;
using IBTSS.Service.DTO.Request.Transaction;
using IBTSS.Service.DTO.Response.Transaction;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IBTSS.Service.Services.TransactionService
{
    public interface ITransactionService
    {
        Task<List<TransactionResponse>> GetAllAsync();
        Task<TransactionResponse?> GetByIdAsync(string id);
        Task<List<TransactionResponse>> GetByCustomerIdAsync(string customerId);
        Task<TransactionResponse> AddAsync(TransactionRequest request);
        Task<TransactionResponse?> UpdateAsync(string id, TransactionRequest request);
        Task<bool> DeleteAsync(string id);
        Task<List<object>> GetRevenueByMonthAsync(int year, int month);
    }
}
