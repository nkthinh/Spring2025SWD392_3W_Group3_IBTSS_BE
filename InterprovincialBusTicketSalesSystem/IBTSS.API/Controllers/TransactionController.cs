using IBTSS.Repository.Entities;
using IBTSS.Service.DTO.Request.Transaction;
using IBTSS.Service.DTO.Request.Trip;
using IBTSS.Service.DTO.Response.Transaction;
using IBTSS.Service.Services.TransactionService;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace IBTSS.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class TransactionController : ControllerBase
    {
        private readonly ITransactionService _transactionService;

        public TransactionController(ITransactionService transactionService)
        {
            _transactionService = transactionService;
        }

        [HttpGet]
        public async Task<IActionResult> GetFiltered([FromQuery] QueryParameters? query)
        {
            try
            {
                if (query == null || (string.IsNullOrEmpty(query.Keyword) && query.PageSize == 0 && query.Page == 0 && string.IsNullOrEmpty(query.SortBy)))
                {
                    var all = await _transactionService.GetAllAsync();
                    return Ok(all);
                }

                if (query.Page == 0) query.Page = 1;
                if (query.PageSize == 0) query.PageSize = 10;

                var (data, totalCount) = await _transactionService.GetFilteredAsync(query);

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
        public async Task<ActionResult<TransactionResponse>> GetById(string id)
        {
            try
            {
                var result = await _transactionService.GetByIdAsync(id);
                if (result == null)
                    return NotFound(new { message = $"Transaction with ID '{id}' not found." });

                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = ex.Message });
            }
        }

        [HttpGet("by-customer/{customerId}")]
        public async Task<ActionResult<List<TransactionResponse>>> GetByCustomerId(string customerId)
        {
            try
            {
                var result = await _transactionService.GetByCustomerIdAsync(customerId);
                if (result == null || !result.Any())
                    return NotFound(new { message = $"No transactions found for customer '{customerId}'." });

                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = ex.Message });
            }
        }

        [HttpPost]
        public async Task<ActionResult<TransactionResponse>> Create([FromBody] TransactionRequest request)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(request.CustomerId))
                    return BadRequest(new { message = "CustomerId is required." });

                var created = await _transactionService.AddAsync(request);
                if (created == null)
                    return BadRequest(new { message = "No pending booking found for the customer." });

                return CreatedAtAction(nameof(GetById), new { id = created.TransactionId }, created);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = ex.Message });
            }
        }

        [HttpPut("{id}")]
        public async Task<ActionResult<TransactionResponse>> Update(string id, [FromBody] TransactionRequest request)
        {
            try
            {
                var updated = await _transactionService.UpdateAsync(id, request);
                if (updated == null)
                    return NotFound(new { message = $"Transaction with ID '{id}' not found." });

                return Ok(updated);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = ex.Message });
            }
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(string id)
        {
            try
            {
                var deleted = await _transactionService.DeleteAsync(id);
                if (!deleted)
                    return NotFound(new { message = $"Transaction with ID '{id}' not found." });

                return NoContent();
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = ex.Message });
            }
        }

        [HttpGet("revenue-by-month")]
        public async Task<IActionResult> GetRevenueByMonth([FromQuery] int year, [FromQuery] int month)
        {
            try
            {
                var result = await _transactionService.GetRevenueByMonthAsync(year, month);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = ex.Message });
            }
        }
    }
}
