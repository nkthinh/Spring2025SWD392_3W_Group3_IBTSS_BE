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
            var result = await _ticketService.GetAllAsync();
            return Ok(result);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<TicketResponse>> GetById(string id)
        {
            var result = await _ticketService.GetByIdAsync(id);
            if (result == null) return NotFound();
            return Ok(result);
        }

        [HttpGet("by-customer/{customerId}")]
        public async Task<IActionResult> GetTicketsByCustomer(string customerId)
        {
            var tickets = await _ticketService.GetByCustomerIdAsync(customerId);
            return Ok(tickets);
        }

        [HttpPost]
        public async Task<ActionResult<TicketResponse>> Create([FromBody] TicketRequest request)
        {
            var created = await _ticketService.AddAsync(request);
            return CreatedAtAction(nameof(GetById), new { id = created.TicketId }, created);
        }

        [HttpPut("{id}")]
        public async Task<ActionResult<TicketResponse>> Update(string id, [FromBody] TicketRequest request)
        {
            var updated = await _ticketService.UpdateAsync(id, request);
            if (updated == null) return NotFound();
            return Ok(updated);
        }
        [HttpPut("cancel/{id}")]
        public async Task<ActionResult<TicketResponse>> CancelTicket(string id)
        {
            var result = await _ticketService.CancelTicketAsync(id);
            if (result == null) return NotFound();
            return Ok(result);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(string id)
        {
            var result = await _ticketService.DeleteAsync(id);
            if (!result) return NotFound();
            return NoContent();
        }
        [HttpPut("change-seat/{ticketId}")]
        public async Task<IActionResult> ChangeSeat(string ticketId, [FromQuery] string newSeatId)
        {
            var result = await _ticketService.ChangeSeatAsync(ticketId, newSeatId);
            return result ? Ok("Seat updated successfully.") : BadRequest("Seat update failed.");
        }

    }
}
