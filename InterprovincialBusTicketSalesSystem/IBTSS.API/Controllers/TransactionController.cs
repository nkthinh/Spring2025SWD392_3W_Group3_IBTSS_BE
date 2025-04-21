using IBTSS.Repository.Entities;
using IBTSS.Service.DTO.Request.Transaction;
using IBTSS.Service.DTO.Response.Transaction;
using IBTSS.Service.Services.TransactionService;
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
        public async Task<ActionResult<List<TransactionResponse>>> GetAll()
        {
            try
            {
                var result = await _transactionService.GetAllAsync();
                return Ok(result);
            }
            catch
            {
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<TransactionResponse>> GetById(string id)
        {
            try
            {
                var result = await _transactionService.GetByIdAsync(id);
                if (result == null)
                    return NotFound();

                return Ok(result);
            }
            catch
            {
                return StatusCode(500, "Internal server error");
            }
        }
        [HttpGet("by-customer/{customerId}")]
        public async Task<ActionResult<List<TransactionResponse>>> GetByCustomerId(string customerId)
        {
            try
            {
                var result = await _transactionService.GetByCustomerIdAsync(customerId);
                if (result == null || !result.Any())
                    return NotFound();

                return Ok(result);
            }
            catch
            {
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpPost]
        public async Task<ActionResult<TransactionResponse>> Create([FromBody] TransactionRequest request)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(request.CustomerId))
                    return BadRequest("CustomerId is required.");

                var created = await _transactionService.AddAsync(request);
                return CreatedAtAction(nameof(GetById), new { id = created.TransactionId }, created);
            }
            catch (Exception ex)
            {
                return StatusCode(500, "An error occurred while creating the transaction.");
            }
        }


        [HttpPut("{id}")]
        public async Task<ActionResult<TransactionResponse>> Update(string CustomerId, [FromBody] TransactionRequest request)
        {
            try
            {
                var updated = await _transactionService.UpdateAsync(CustomerId, request);
                if (updated == null)
                    return NotFound();

                return Ok(updated);
            }
            catch
            {
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(string id)
        {
            try
            {
                var deleted = await _transactionService.DeleteAsync(id);
                if (!deleted)
                    return NotFound();

                return NoContent();
            }
            catch
            {
                return StatusCode(500, "Internal server error");
            }
        }
        [HttpGet("unpaid-tickets/{customerId}")]
        public async Task<ActionResult<List<Book>>> GetUnpaidTickets(string customerId)
        {
            try
            {
                var tickets = await _transactionService.GetUnpaidTicketsByCustomerIdAsync(customerId);
                return Ok(tickets);
            }
            catch
            {
                return StatusCode(500, "Internal server error");
            }
        }

    }
}
