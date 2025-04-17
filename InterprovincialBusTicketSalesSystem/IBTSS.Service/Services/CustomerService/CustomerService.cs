using IBTSS.Repository.Entities;
using IBTSS.Repository.UnitOfWork;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata;
using System.Text;
using System.Threading.Tasks;

namespace IBTSS.Service.Services.CustomerService
{
    public class CustomerService(IUnitOfWork _unitOfWork, ILogger<CustomerService> _logger) : ICustomerService
    {
        public async Task<IEnumerable<Customer>> GetAllAsync()
        {
            try
            {
                _logger.LogInformation("Fetching all Customers.");
                return await _unitOfWork.Customers.GetAllAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching all Customers.");
                throw new Exception("An error occurred while retrieving Customers.", ex);
            }
        }
        public async Task AddAsync(Customer c)
        {
            try
            {
                _logger.LogInformation("Adding a new Customer");
                await _unitOfWork.Customers.AddAsync(c);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error adding Customer");
                throw;
            }
        }
    }
}
