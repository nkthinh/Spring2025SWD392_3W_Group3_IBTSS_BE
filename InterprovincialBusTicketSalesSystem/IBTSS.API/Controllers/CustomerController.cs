using AutoMapper;
using IBTSS.Repository.Entities;
using IBTSS.Service.DTO.Request;
using IBTSS.Service.DTO.Response;
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
                    return NotFound(new { message = "No blog contents found." });
                }

                var customerResponse = mapper.Map<IEnumerable<CustomerResponse>>(customers);
                return Ok(customerResponse);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }
        [HttpPost]
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
    }
}
