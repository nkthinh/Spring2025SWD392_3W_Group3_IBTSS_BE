using AutoMapper;
using IBTSS.Repository.Entities;
using IBTSS.Service.DTO.Request.Customer;
using IBTSS.Service.DTO.Response.Customer;
using IBTSS.Service.Services.CustomerService;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace IBTSS.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class CustomerController(IMapper mapper, ICustomerService customerService) : ControllerBase
    {
        [HttpGet]
        public async Task<IActionResult> GetFiltered([FromQuery] CustomerQueryParameters? query)
        {
            try
            {
                if (query == null || (string.IsNullOrEmpty(query.Keyword) && string.IsNullOrEmpty(query.SortBy) && query.Page == 0 && query.PageSize == 0))
                {
                    var all = await customerService.GetAllAsync();
                    var mapped = mapper.Map<IEnumerable<CustomerResponse>>(all);
                    return Ok(mapped);
                }

                if (query.Page == 0) query.Page = 1;
                if (query.PageSize == 0) query.PageSize = 10;

                var (data, totalCount) = await customerService.GetFilteredAsync(query);

                var pagination = new
                {
                    TotalCount = totalCount,
                    PageSize = query.PageSize,
                    CurrentPage = query.Page,
                    TotalPages = (int)Math.Ceiling((double)totalCount / query.PageSize)
                };

                return Ok(new { Data = data, Pagination = pagination });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = ex.Message });
            }
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(string id)
        {
            var customer = await customerService.GetByIdAsync(id);
            if (customer == null)
                return NotFound(new { message = "Customer not found." });

            var customerResponse = mapper.Map<CustomerResponse>(customer);
            return Ok(customerResponse);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(string id, [FromBody] CustomerRequest request)
        {
            var existing = await customerService.GetByIdAsync(id);
            if (existing == null)
                return NotFound(new { message = "Customer not found." });

            var updatedCustomer = mapper.Map(request, existing);
            await customerService.UpdateAsync(updatedCustomer);
            return Ok(new { message = "Customer updated successfully." });
        }

        [Authorize(Roles = "Admin")]
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(string id)
        {
            var existing = await customerService.GetByIdAsync(id);
            if (existing == null)
                return NotFound(new { message = "Customer not found." });

            await customerService.DeleteAsync(id);
            return Ok(new { message = "Customer deleted successfully." });
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] CustomerRequest request)
        {
            try
            {
                var isUnique = await customerService.GetByPhoneNumberAsync(request.PhoneNumber);
                if (!isUnique)
                    return BadRequest(new { message = "Phone number already exists." });

                var customer = mapper.Map<Customer>(request);
                customer.PasswordHash = customerService.HashPassword(request.Password);
                await customerService.AddAsync(customer);

                // ✅ Load lại từ database kèm Membership
                var createdCustomer = await customerService.GetByIdAsync(customer.CustomerId);

                var customerResponse = mapper.Map<CustomerResponse>(createdCustomer);
                return Ok(customerResponse);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = ex.Message });
            }
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] CustomerLoginRequest customerLoginRequest)
        {
            try
            {
                var customer = await customerService.AuthenticateAsync(customerLoginRequest.PhoneNumber, customerLoginRequest.Password);
                if (customer == null)
                {
                    return Unauthorized(new { message = "Invalid phone number or password." });
                }
                // ✅ Load lại từ database kèm Membership
                var createdCustomer = await customerService.GetByIdAsync(customer.CustomerId);
                var customerResponse = mapper.Map<CustomerResponse>(customer);
                return Ok(customerResponse);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = ex.Message });
            }
        }


        [HttpGet("membership/{customerId}")]
        public async Task<IActionResult> GetCustomerMembership(string customerId)
        {
            var customer = await customerService.GetByIdAsync(customerId);
            if (customer == null)
                return NotFound(new { message = "Customer not found." });

            var membership = customer.Membership;

            return Ok(new
            {
                customerId = customer.CustomerId,
                name = customer.Name,
                score = customer.Score,
                rank = membership?.RankName ?? "Default",
                discountRate = membership?.DiscountRate ?? 0,
                discountQuotaLeft = customer?.DiscountQuotaLeft ?? 0
            });
        }
    }
}