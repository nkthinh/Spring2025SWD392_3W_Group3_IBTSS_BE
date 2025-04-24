using IBTSS.Service.DTO.Request.Ticket;
using IBTSS.Service.DTO.Response.Ticket;
using IBTSS.Service.Services.TicketService;
using Microsoft.AspNetCore.Mvc;

namespace IBTSS.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class TicketController : ControllerBase
    {
        private readonly ITicketService _ticketService;

        public TicketController(ITicketService ticketService)
        {
            _ticketService = ticketService;
        }

        [HttpGet]
        public async Task<ActionResult<List<TicketResponse>>> GetAll()
        {
            try
            {
                var result = await _ticketService.GetAllAsync();
                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = ex.Message });
            }
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<TicketResponse>> GetById(string id)
        {
            try
            {
                var result = await _ticketService.GetByIdAsync(id);
                if (result == null)
                    return NotFound(new { message = $"Ticket with ID '{id}' not found." });

                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = ex.Message });
            }
        }

        [HttpGet("by-customer/{customerId}")]
        public async Task<IActionResult> GetTicketsByCustomer(string customerId)
        {
            try
            {
                var tickets = await _ticketService.GetByCustomerIdAsync(customerId);
                return Ok(tickets);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = ex.Message });
            }
        }

        [HttpPost]
        public async Task<ActionResult<TicketResponse>> Create([FromBody] TicketRequest request)
        {
            try
            {
                var created = await _ticketService.AddAsync(request);
                return CreatedAtAction(nameof(GetById), new { id = created.TicketId }, created);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = ex.Message });
            }
        }

        [HttpPut("{id}")]
        public async Task<ActionResult<TicketResponse>> Update(string id, [FromBody] TicketRequest request)
        {
            try
            {
                var updated = await _ticketService.UpdateAsync(id, request);
                if (updated == null)
                    return NotFound(new { message = $"Ticket with ID '{id}' not found." });

                return Ok(updated);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = ex.Message });
            }
        }

        [HttpPut("cancel/{id}")]
        public async Task<ActionResult<TicketResponse>> CancelTicket(string id)
        {
            try
            {
                var result = await _ticketService.CancelTicketAsync(id);
                if (result == null)
                    return NotFound(new { message = $"Ticket with ID '{id}' not found." });

                return Ok(result);
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
                var result = await _ticketService.DeleteAsync(id);
                if (!result)
                    return NotFound(new { message = $"Ticket with ID '{id}' not found." });

                return NoContent();
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = ex.Message });
            }
        }
    }
}
