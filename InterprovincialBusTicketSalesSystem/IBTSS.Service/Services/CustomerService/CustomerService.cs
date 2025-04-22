using IBTSS.Repository.Entities;
using IBTSS.Repository.UnitOfWork;
using Microsoft.Extensions.Logging;

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

        public async Task<Customer?> GetByIdAsync(string id)
        {
            try
            {
                return await _unitOfWork.Customers.GetByIdAsync(id);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error fetching Customer with id: {id}");
                throw;
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

        public async Task UpdateAsync(Customer c)
        {
            try
            {
                _logger.LogInformation($"Updating Customer {c.CustomerId}");
                await _unitOfWork.Customers.UpdateAsync(c);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error updating Customer {c.CustomerId}");
                throw;
            }
        }
        public async Task<bool> GetByPhoneNumberAsync(string phoneNumber)
        {
            try
            {
                return await _unitOfWork.Customers.GetByPhoneNumberAsync(phoneNumber);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error checking phone number");
                throw;
            }
        }

        public async Task<Customer?> LoginByPhoneAsync(string phoneNumber)
        {
            try
            {
                return await _unitOfWork.Customers.LoginByPhoneAsync(phoneNumber);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "PhoneNumber is not exist!");
                throw;
            }
        }

        public async Task DeleteAsync(string id)
        {
            try
            {
                _logger.LogInformation($"Deleting Customer {id}");
                await _unitOfWork.Customers.DeleteAsync(id);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error deleting Customer {id}");
                throw;
            }
        }
    }
}