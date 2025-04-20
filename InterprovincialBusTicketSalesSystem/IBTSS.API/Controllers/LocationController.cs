using IBTSS.Service.DTO.Request.Location;
using IBTSS.Service.DTO.Response.Location;
using IBTSS.Service.Services.LocationService;
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

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var locations = await _locationService.GetAllAsync();
            return Ok(locations);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(string id)
        {
            var location = await _locationService.GetByIdAsync(id);
            if (location == null) return NotFound($"Location with ID {id} not found.");
            return Ok(location);
        }

        [HttpPost]
        public async Task<IActionResult> Add(LocationRequest request)
        {
            var created = await _locationService.AddAsync(request);
            return CreatedAtAction(nameof(GetById), new { id = created.LocationId }, created);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(string id, LocationRequest request)
        {
            var updated = await _locationService.UpdateAsync(id, request);
            if (updated == null) return NotFound($"Location with ID {id} not found.");
            return Ok(updated);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(string id)
        {
            var deleted = await _locationService.DeleteAsync(id);
            if (!deleted) return NotFound($"Location with ID {id} not found.");
            return NoContent();
        }
    }
}
