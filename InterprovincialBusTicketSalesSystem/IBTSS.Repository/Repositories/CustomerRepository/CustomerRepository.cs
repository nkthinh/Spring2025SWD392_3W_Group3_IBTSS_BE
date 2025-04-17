using IBTSS.Repository.Entities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata;
using System.Text;
using System.Threading.Tasks;

namespace IBTSS.Repository.Repositories.CustomerRepository
{
    public class CustomerRepository : ICustomerRepository
    {
        private readonly AppDbContext _context;
        public CustomerRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task AddAsync(Customer c)
        {
            await _context.Customers.AddAsync(c);
            await _context.SaveChangesAsync();
        }
        public async Task<IEnumerable<Customer>> GetAllAsync()
        {
            return await _context.Customers
                .Include(c => c.Membership)
                .ToListAsync();
        }
        public async Task<Customer?> GetByIdAsync(string id)
        {
            return await _context.Customers
                .Include(c => c.Membership)
                .FirstOrDefaultAsync(c => c.CustomerId == id);
        }
        public async Task UpdateAsync(Customer c)
        {
            _context.Customers.Update(c);
            await _context.SaveChangesAsync();
        }
        public async Task DeleteAsync(string id)
        {
            var customer = await _context.Customers.FindAsync(id);
            if (customer != null)
            {
                _context.Customers.Remove(customer);
                await _context.SaveChangesAsync();
            }
        }
        public async Task<Customer?> GetByPhoneNumberAsync(string phoneNumber)
        {
            return await _context.Customers
                .Include(c => c.Membership)
                .FirstOrDefaultAsync(c => c.PhoneNumber == phoneNumber);
        }
        //public async Task SoftDeleteAsync(string id)
        //{
        //    var customer = await _context.Customers.FindAsync(id);
        //    if (customer != null)
        //    {
        //        customer.isDelete = true;
        //        await _context.SaveChangesAsync();
        //    }
        //}
        //public async Task RestoreAsync(string id)
        //{
        //    var customer = await _context.Customers.FindAsync(id);
        //    if (customer != null)
        //    {
        //        customer.isDelete = false;
        //        await _context.SaveChangesAsync();
        //    }
        //}
    }
}
