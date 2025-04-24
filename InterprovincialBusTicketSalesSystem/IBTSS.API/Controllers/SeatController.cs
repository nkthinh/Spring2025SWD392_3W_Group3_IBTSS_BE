using IBTSS.Service.DTO.Request.Seat;
using IBTSS.Service.DTO.Response.Seat;
using IBTSS.Service.Services.SeatService;
using Microsoft.AspNetCore.Mvc;

namespace IBTSS.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class SeatController : ControllerBase
    {
        private readonly ISeatService _seatService;

        public SeatController(ISeatService seatService)
        {
            _seatService = seatService;
        }

        [HttpGet]
        public async Task<ActionResult<List<SeatResponse>>> GetAll()
        {
            try
            {
                var result = await _seatService.GetAllAsync();
                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = ex.Message });
            }
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<SeatResponse>> GetById(string id)
        {
            try
            {
                var result = await _seatService.GetByIdAsync(id);
                if (result == null)
                    return NotFound(new { message = $"Seat with ID '{id}' not found." });

                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = ex.Message });
            }
        }

        [HttpPost]
        public async Task<ActionResult<SeatResponse>> Create([FromBody] SeatRequest request)
        {
            try
            {
                var created = await _seatService.AddAsync(request);
                return CreatedAtAction(nameof(GetById), new { id = created.SeatId }, created);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = ex.Message });
            }
        }

        [HttpPut("{id}")]
        public async Task<ActionResult<SeatResponse>> Update(string id, [FromBody] SeatRequest request)
        {
            try
            {
                var updated = await _seatService.UpdateAsync(id, request);
                if (updated == null)
                    return NotFound(new { message = $"Seat with ID '{id}' not found." });

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
                var result = await _seatService.DeleteAsync(id);
                if (!result)
                    return NotFound(new { message = $"Seat with ID '{id}' not found." });

                return NoContent();
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = ex.Message });
            }
        }

        [HttpGet("trip/{tripId}/availability")]
        public async Task<ActionResult<SeatSummaryResponse>> GetAvailabilityByTrip(string tripId)
        {
            try
            {
                var result = await _seatService.GetSeatAvailabilityByTripIdAsync(tripId);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = ex.Message });
            }
        }
    }
}
