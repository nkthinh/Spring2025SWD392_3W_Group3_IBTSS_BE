using IBTSS.Service.DTO.Request.Location;
using IBTSS.Service.DTO.Response.Location;
using IBTSS.Service.Services.LocationService;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace IBTSS.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class LocationController : ControllerBase
    {
        private readonly ILocationService _locationService;

        public LocationController(ILocationService locationService)
        {
            _locationService = locationService;
        }
        //[Authorize(Roles = "Admin")]
        //[HttpGet]
        //public async Task<IActionResult> GetAll([FromQuery] LocationQueryParameters? query)
        //{
        //    if (query == null || (string.IsNullOrEmpty(query.Keyword) && string.IsNullOrEmpty(query.SortBy) && query.Page == 0 && query.PageSize == 0))
        //    {
        //        var locations = await _locationService.GetAllAsync();
        //        return Ok(locations);
        //    }

        //    if (query.Page == 0) query.Page = 1;
        //    if (query.PageSize == 0) query.PageSize = 10;

        //    var (data, total) = await _locationService.GetFilteredAsync(query);

        //    return Ok(new
        //    {
        //        Data = data,
        //        Pagination = new
        //        {
        //            TotalCount = total,
        //            PageSize = query.PageSize,
        //            CurrentPage = query.Page,
        //            TotalPages = (int)Math.Ceiling((double)total / query.PageSize)
        //        }
        //    });
        //}

        [Authorize(Roles = "Admin")]
        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(string id)
        {
            var location = await _locationService.GetByIdAsync(id);
            if (location == null) return NotFound($"Location with ID {id} not found.");
            return Ok(location);
        }
        [Authorize(Roles = "Admin")]
        [HttpPost]
        public async Task<IActionResult> Add(LocationRequest request)
        {
            var created = await _locationService.AddAsync(request);
            return CreatedAtAction(nameof(GetById), new { id = created.LocationId }, created);
        }
        [Authorize(Roles = "Admin")]
        [HttpPut("{id}")]
        public async Task<IActionResult> Update(string id, LocationRequest request)
        {
            var updated = await _locationService.UpdateAsync(id, request);
            if (updated == null) return NotFound($"Location with ID {id} not found.");
            return Ok(updated);
        }
        [Authorize(Roles = "Admin")]
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(string id)
        {
            var deleted = await _locationService.DeleteAsync(id);
            if (!deleted) return NotFound($"Location with ID {id} not found.");
            return NoContent();
        }
    }
}
