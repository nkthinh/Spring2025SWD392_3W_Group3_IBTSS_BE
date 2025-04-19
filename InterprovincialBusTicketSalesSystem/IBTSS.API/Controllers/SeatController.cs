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
            var result = await _seatService.GetAllAsync();
            return Ok(result);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<SeatResponse>> GetById(string id)
        {
            var result = await _seatService.GetByIdAsync(id);
            if (result == null) return NotFound();
            return Ok(result);
        }

        [HttpPost]
        public async Task<ActionResult<SeatResponse>> Create([FromBody] SeatRequest request)
        {
            var created = await _seatService.AddAsync(request);
            return CreatedAtAction(nameof(GetById), new { id = created.SeatId }, created);
        }

        [HttpPut("{id}")]
        public async Task<ActionResult<SeatResponse>> Update(string id, [FromBody] SeatRequest request)
        {
            var updated = await _seatService.UpdateAsync(id, request);
            if (updated == null) return NotFound();
            return Ok(updated);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(string id)
        {
            var result = await _seatService.DeleteAsync(id);
            if (!result) return NotFound();
            return NoContent();
        }
        [HttpGet("bus/{busId}/availability")]
        public async Task<ActionResult<SeatSummaryResponse>> GetAvailability(string busId)
        {
            var result = await _seatService.GetSeatAvailabilityByBusIdAsync(busId);
            return Ok(result);
        }

    }
}
