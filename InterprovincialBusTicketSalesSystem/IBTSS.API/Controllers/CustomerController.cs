using AutoMapper;
using IBTSS.Repository.Entities;
using IBTSS.Service.DTO.Request.Customer;
using IBTSS.Service.DTO.Response.Customer;
using IBTSS.Service.Services.CustomerService;
using Microsoft.AspNetCore.Mvc;

namespace IBTSS.API.Controllers
{
    [Route("customers")]
    [ApiController]
    public class CustomerController(IMapper mapper, ICustomerService customerService) : ControllerBase
    {
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            try
            {
                var customers = await customerService.GetAllAsync();
                if (!customers.Any())
                {
                    return NotFound(new { message = "No customers found." });
                }

                var customerResponse = mapper.Map<IEnumerable<CustomerResponse>>(customers);
                return Ok(customerResponse);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = ex.Message });
            }
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] CustomerRequest customerRequest)
        {
            try
            {
                var customer = mapper.Map<Customer>(customerRequest);
                if (customer == null)
                {
                    return BadRequest(new { message = "Customer data is invalid." });
                }

                var isUnique = await customerService.GetByPhoneNumberAsync(customer.PhoneNumber);
                if (!isUnique)
                {
                    return BadRequest(new { message = "Phone number already exists." });
                }

                await customerService.AddAsync(customer);
                var customerResponse = mapper.Map<CustomerResponse>(customer);
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
                return StatusCode(500, new { message = ex.Message });
            }
        }
    }
}
