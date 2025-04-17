using IBTSS.Repository.Repositories.CustomerRepository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IBTSS.Repository.UnitOfWork
{
    public class UnitOfWork(AppDbContext _context, ICustomerRepository _customerRepository) : IUnitOfWork
    {
        public ICustomerRepository Customers { get; } = _customerRepository;


        public async Task<int> CompleteAsync()
        {
            return await _context.SaveChangesAsync();
        }

        public void Dispose()
        {
            _context.Dispose();
        }
    }
}
