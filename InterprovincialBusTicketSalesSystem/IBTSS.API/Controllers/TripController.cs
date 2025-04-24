using IBTSS.Service.DTO.Request.Trip;
using IBTSS.Service.DTO.Response.Trip;
using IBTSS.Service.Services.TripService;
using Microsoft.AspNetCore.Mvc;

namespace IBTSS.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class TripController : ControllerBase
    {
        private readonly ITripService _tripService;

        public TripController(ITripService tripService)
        {
            _tripService = tripService;
        }

        [HttpGet]
        public async Task<IActionResult> GetFiltered([FromQuery] QueryParameters? query)
        {
            try
            {
                if (query == null ||
                    (string.IsNullOrEmpty(query.Keyword) && string.IsNullOrEmpty(query.SortBy) && query.Page == 0 && query.PageSize == 0))
                {
                    var allTrips = await _tripService.GetAllAsync();
                    return Ok(allTrips);
                }

                if (query.Page == 0) query.Page = 1;
                if (query.PageSize == 0) query.PageSize = 10;

                var (data, totalCount) = await _tripService.GetFilteredAsync(query);

                var pagination = new
                {
                    TotalCount = totalCount,
                    PageSize = query.PageSize,
                    CurrentPage = query.Page,
                    TotalPages = query.PageSize == -1 ? 1 : (int)Math.Ceiling((double)totalCount / query.PageSize)
                };

                return Ok(new { Data = data, Pagination = pagination });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = ex.Message });
            }
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<TripResponse>> GetById(string id)
        {
            try
            {
                var trip = await _tripService.GetByIdAsync(id);
                if (trip == null)
                    return NotFound(new { message = $"Trip with ID '{id}' not found." });

                return Ok(trip);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = ex.Message });
            }
        }

        [HttpPost]
        public async Task<ActionResult<TripResponse>> Create([FromBody] TripRequest request)
        {
            try
            {
                var created = await _tripService.AddAsync(request);
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
        public async Task<ActionResult<TripResponse>> Update(string id, [FromBody] TripRequest request)
        {
            try
            {
                var updated = await _tripService.UpdateAsync(id, request);
                if (updated == null)
                    return NotFound(new { message = $"Trip with ID '{id}' not found." });

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
                var result = await _tripService.DeleteAsync(id);
                if (!result)
                    return NotFound(new { message = $"Trip with ID '{id}' not found." });

                return Ok(new { message = "Deleted successfully" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = ex.Message });
            }
        }

        [HttpGet("search-by-date")]
        public async Task<ActionResult<IEnumerable<TripSearchDto>>> SearchByDate([FromQuery] string date)
        {
            try
            {
                var results = await _tripService.SearchByDateAsync(date);
                return Ok(results);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = ex.Message });
            }
        }

        [HttpGet("search-by-location-and-route")]
        public async Task<IActionResult> SearchTripsByKeyword([FromQuery] string keyword)
        {
            try
            {
                var results = await _tripService.SearchByKeywordAsync(keyword);
                return Ok(results);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = ex.Message });
            }
        }

        [HttpGet("search")]
        public async Task<ActionResult<IEnumerable<TripSearchDto>>> SearchTrips(
            [FromQuery] string keyword,
            [FromQuery] string? date,
            [FromQuery] string? type)
        {
            try
            {
                var result = await _tripService.SearchTripsAsync(keyword, date, type);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = ex.Message });
            }
        }

        [HttpGet("assigned/{driverId}")]
        public async Task<ActionResult<List<TripResponse>>> GetTripsByDriver(string driverId)
        {
            try
            {
                var result = await _tripService.GetTripsByDriverIdAsync(driverId);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = ex.Message });
            }
        }

        [HttpGet("{tripId}/customers")]
        public async Task<IActionResult> GetCustomersByTrip(string tripId)
        {
            try
            {
                var result = await _tripService.GetCustomersByTripAsync(tripId);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = ex.Message });
            }
        }

        [HttpPut("complete/{tripId}")]
        public async Task<IActionResult> CompleteTrip(string tripId)
        {
            try
            {
                var result = await _tripService.CompleteTripAsync(tripId);
                if (result == null)
                    return NotFound(new { message = $"Trip with ID '{tripId}' not found or already completed." });

                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = ex.Message });
            }
        }

        [HttpGet("calendar")]
        public async Task<IActionResult> GetTripsForCalendar([FromQuery] string? month)
        {
            try
            {
                var data = await _tripService.GetTripsForCalendarAsync(month);
                return Ok(data);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = ex.Message });
            }
        }
    }
}
