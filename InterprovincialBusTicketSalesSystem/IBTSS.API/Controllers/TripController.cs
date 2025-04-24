using IBTSS.Service.DTO.Request.Trip;
using IBTSS.Service.DTO.Response.Trip;
using IBTSS.Service.Services.TripService;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace IBTSS.API.Controllers
{


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

            //[HttpGet]
            //public async Task<ActionResult<List<TripResponse>>> GetAll()
            //{
            //    var trips = await _tripService.GetAllAsync();
            //    return Ok(trips);
            //}
            //

            [HttpGet]
            public async Task<IActionResult> Get([FromQuery] QueryParameters? query)
            {
                // Nếu không truyền gì hoặc toàn bộ là mặc định → GetAll
                if (query == null ||
                    (string.IsNullOrEmpty(query.Keyword) && string.IsNullOrEmpty(query.SortBy) && query.Page == 0 && query.PageSize == 0))
                {
                    var allTrips = await _tripService.GetAllAsync();
                    return Ok(allTrips);
                }

                // Nếu không có Page hoặc PageSize thì gán mặc định
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

                return Ok(new
                {
                    Data = data,
                    Pagination = pagination
                });
            }

            [HttpGet("{id}")]
            public async Task<ActionResult<TripResponse>> GetById(string id)
            {
                var trip = await _tripService.GetByIdAsync(id);
                if (trip == null) return NotFound();
                return Ok(trip);
            }

            [HttpPost]
            public async Task<ActionResult<TripResponse>> Create([FromBody] TripRequest request)
            {
                var created = await _tripService.AddAsync(request);
                return CreatedAtAction(nameof(GetById), new { id = created.TripId }, created);
            }

            [HttpPut("{id}")]
            public async Task<ActionResult<TripResponse>> Update(string id, [FromBody] TripRequest request)
            {
                var updated = await _tripService.UpdateAsync(id, request);
                if (updated == null) return NotFound();
                return Ok(updated);
            }

            [HttpDelete("{id}")]
            public async Task<IActionResult> Delete(string id)
            {
                var result = await _tripService.DeleteAsync(id);
                if (!result) return NotFound();
                return NoContent();
            }
            [HttpGet("search-by-date")]
            public async Task<ActionResult<IEnumerable<TripSearchDto>>> SearchByDate([FromQuery] string date)
            {
                var results = await _tripService.SearchByDateAsync(date);
                return Ok(results);
            }
            ///search-by-location-and-route
            [HttpGet("search-by-location-and-route")]
            public async Task<IActionResult> SearchTripsByKeyword([FromQuery] string keyword)
            {
                var results = await _tripService.SearchByKeywordAsync(keyword);
                return Ok(results);
            }


            [HttpGet("search")]
            public async Task<ActionResult<IEnumerable<TripSearchDto>>> SearchTrips(
         [FromQuery] string keyword,
         [FromQuery] string date,
         [FromQuery] string type)
            {
                var result = await _tripService.SearchTripsAsync(keyword, date, type);
                return Ok(result);
            }

            [HttpGet("assigned/{driverId}")]
            public async Task<ActionResult<List<TripResponse>>> GetTripsByDriver(string driverId)
            {
                var result = await _tripService.GetTripsByDriverIdAsync(driverId);
                return Ok(result);
            }
            [HttpGet("{tripId}/customers")]
            public async Task<IActionResult> GetCustomersByTrip(string tripId)
            {
                var result = await _tripService.GetCustomersByTripAsync(tripId);
                return Ok(result);
            }

            [HttpPut("complete/{tripId}")]
            public async Task<IActionResult> CompleteTrip(string tripId)
            {
                var result = await _tripService.CompleteTripAsync(tripId);
                if (result == null) return NotFound();
                return Ok(result);
            }
            [HttpGet("calendar")]
            public async Task<IActionResult> GetTripsForCalendar([FromQuery] string? month)
            {
                var data = await _tripService.GetTripsForCalendarAsync(month);
                return Ok(data);
            }


        }
    }

}
