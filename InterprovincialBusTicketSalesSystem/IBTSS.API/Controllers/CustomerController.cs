using AutoMapper;
using IBTSS.Repository.Entities;
using IBTSS.Service.DTO.Request.Customer;
using IBTSS.Service.DTO.Response.Customer;
using IBTSS.Service.Services.CustomerService;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity.Data;
using Microsoft.AspNetCore.Mvc;

namespace IBTSS.API.Controllers
{
    [Route("customers")]
    [ApiController]
    public class CustomerController(IMapper mapper, ICustomerService customerService) : ControllerBase
    {
        [Authorize(Roles = "Admin")]
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            try
            {
                var customers = await customerService.GetAllAsync();
                if (!customers.Any())
                {
                    return NotFound(new { message = "No blog contents found." });
                }

                var customerResponse = mapper.Map<IEnumerable<CustomerResponse>>(customers);
                return Ok(customerResponse); // 200 OK
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}"); // 500 Error
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

        [Authorize(Roles = "Admin")]
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
        public async Task<IActionResult> Resgister([FromBody] CustomerRequest customerrequest)
        {
            try
            {
                var customer = mapper.Map<Customer>(customerrequest);
                if (customer == null)
                {
                    return BadRequest("Customer is null");
                }
                // Kiểm tra số điện thoại đã tồn tại chưa
                var isUnique = await customerService.GetByPhoneNumberAsync(customer.PhoneNumber);
                if (!isUnique)
                {
                    return BadRequest(new { message = "PhoneNumber is extisted." });
                }
                await customerService.AddAsync(customer);
                var customerResponse = mapper.Map<CustomerResponse>(customer);
                return Ok(customerResponse);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }

        }
        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] CustomerLoginRequest customerLoginRequest)
        {
            try
            {
                var customer = await customerService.LoginByPhoneAsync(customerLoginRequest.PhoneNumber);
                if (customer == null)
                {
                    return Unauthorized(new { message = "Invalid phone number." });
                }

                var customerResponse = mapper.Map<CustomerResponse>(customer);
                return Ok(customerResponse);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
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
                discountRate = membership?.DiscountRate ?? 0
            });
        }

    }
}
