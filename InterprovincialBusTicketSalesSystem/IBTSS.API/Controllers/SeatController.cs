using IBTSS.Service.DTO.Request.Seat;
using IBTSS.Service.DTO.Request.Trip;
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
        public async Task<IActionResult> Get([FromQuery] QueryParameters? query)
        {
            if (query == null ||
                (string.IsNullOrEmpty(query.Keyword) && string.IsNullOrEmpty(query.SortBy) && query.Page == 0 && query.PageSize == 0))
            {
                var seats = await _seatService.GetAllAsync();
                return Ok(seats);
            }

            if (query.Page == 0) query.Page = 1;
            if (query.PageSize == 0) query.PageSize = 10;

            var (data, total) = await _seatService.GetFilteredAsync(query);

            var pagination = new
            {
                TotalCount = total,
                PageSize = query.PageSize,
                CurrentPage = query.Page,
                TotalPages = query.PageSize == -1 ? 1 : (int)Math.Ceiling((double)total / query.PageSize)
            };

            return Ok(new
            {
                Data = data,
                Pagination = pagination
            });
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
                return NotFound(ex.Message);
            }
        }


    }
}
