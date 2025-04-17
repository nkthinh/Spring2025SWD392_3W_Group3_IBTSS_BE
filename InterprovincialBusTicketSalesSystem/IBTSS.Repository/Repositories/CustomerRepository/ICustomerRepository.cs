using IBTSS.Repository.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IBTSS.Repository.Repositories.CustomerRepository
{
    public interface ICustomerRepository
    {
        Task AddAsync(Customer c);
        Task<IEnumerable<Customer>> GetAllAsync();
        Task<Customer?> GetByIdAsync(string id);
        Task UpdateAsync(Customer c);
        Task DeleteAsync(string id);
        Task<bool> GetByPhoneNumberAsync(string phoneNumber); // ✅ Trả về bool
        Task<Customer?> LoginByPhoneAsync(string phoneNumber);
        //Task SoftDeleteAsync(string id);
    }
}
