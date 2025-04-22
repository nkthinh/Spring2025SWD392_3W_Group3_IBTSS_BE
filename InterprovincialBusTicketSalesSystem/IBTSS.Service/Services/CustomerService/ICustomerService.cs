using IBTSS.Repository.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IBTSS.Service.Services.CustomerService
{
    public interface ICustomerService
    {
        Task<IEnumerable<Customer>> GetAllAsync();
        Task<Customer?> GetByIdAsync(string id);
        Task AddAsync(Customer c);
        Task UpdateAsync(Customer c);
        Task DeleteAsync(string id);
        Task<bool> GetByPhoneNumberAsync(string phoneNumber);
        Task<Customer?> LoginByPhoneAsync(string phoneNumber);
    }

}
