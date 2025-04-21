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

            [HttpGet]
            public async Task<ActionResult<List<TripResponse>>> GetAll()
            {
                var trips = await _tripService.GetAllAsync();
                return Ok(trips);
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
            //[HttpGet("search-by-date")]
            //public async Task<ActionResult<IEnumerable<TripSearchDto>>> SearchByDate([FromQuery] string date)
            //{
            //    var results = await _tripService.SearchByDateAsync(date);
            //    return Ok(results);
            //}
            /////search-by-location-and-route
            //[HttpGet("search-by-location-and-route")]
            //public async Task<IActionResult> SearchTripsByKeyword([FromQuery] string keyword)
            //{
            //    var results = await _tripService.SearchByKeywordAsync(keyword);
            //    return Ok(results);
            //}


            [HttpGet("search")]
            public async Task<ActionResult<IEnumerable<TripSearchDto>>> SearchTrips(
         [FromQuery] string keyword,
         [FromQuery] string date,
         [FromQuery] string type)
            {
                var result = await _tripService.SearchTripsAsync(keyword, date, type);
                return Ok(result);
            }


        }
    }

}
