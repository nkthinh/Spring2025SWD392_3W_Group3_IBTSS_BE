using IBTSS.Service.DTO.Request.Ticket;
using IBTSS.Service.DTO.Request.Trip;
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
        public async Task<IActionResult> GetFiltered([FromQuery] QueryParameters? query)
        {
            try
            {
                if (query == null || (string.IsNullOrEmpty(query.Keyword) && string.IsNullOrEmpty(query.SortBy) && query.Page == 0 && query.PageSize == 0))
                {
                    var all = await _ticketService.GetAllAsync();
                    return Ok(all);
                }

                if (query.Page == 0) query.Page = 1;
                if (query.PageSize == 0) query.PageSize = 10;

                var (data, total) = await _ticketService.GetFilteredAsync(query);

                var pagination = new
                {
                    TotalCount = total,
                    PageSize = query.PageSize,
                    CurrentPage = query.Page,
                    TotalPages = query.PageSize == -1 ? 1 : (int)Math.Ceiling((double)total / query.PageSize)
                };

                return Ok(new { Data = data, Pagination = pagination });
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
                return Ok(new
                {
                    message = "Created successfully",
                    data = created
                });
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

        [HttpPut("change-seat/{ticketId}")]
        public async Task<IActionResult> ChangeSeat(string ticketId, [FromQuery] string newSeatId)
        {
            try
            {
                var result = await _ticketService.ChangeSeatAsync(ticketId, newSeatId);
                return result ? Ok("Seat updated successfully.") : BadRequest("Seat update failed.");
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

                return Ok(new { message = "Deleted successfully" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = ex.Message });
            }
        } 
        //Thống kê lượt đặt vé theo Route
        [HttpGet("route-statistics")]
        public async Task<IActionResult> GetRouteBookingStatistics([FromQuery] int year, [FromQuery] int? month, [FromQuery] string sortOrder = "desc")
        {
            try
            {
                var result = await _ticketService.GetRouteBookingStatisticsAsync(year, month, sortOrder);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = ex.Message });
            }
        }
    }
}
