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
            try
            {
                var locations = await _locationService.GetAllAsync();
                return Ok(locations);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = ex.Message });
            }
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(string id)
        {
            try
            {
                var location = await _locationService.GetByIdAsync(id);
                if (location == null)
                    return NotFound(new { message = $"Location with ID '{id}' not found." });

                return Ok(location);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = ex.Message });
            }
        }

        [HttpPost]
        public async Task<IActionResult> Add([FromBody] LocationRequest request)
        {
            try
            {
                var created = await _locationService.AddAsync(request);
                return CreatedAtAction(nameof(GetById), new { id = created.LocationId }, created);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = ex.Message });
            }
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(string id, [FromBody] LocationRequest request)
        {
            try
            {
                var updated = await _locationService.UpdateAsync(id, request);
                if (updated == null)
                    return NotFound(new { message = $"Location with ID '{id}' not found." });

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
                var deleted = await _locationService.DeleteAsync(id);
                if (!deleted)
                    return NotFound(new { message = $"Location with ID '{id}' not found." });

                return NoContent(); // 204 No Content
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = ex.Message });
            }
        }
    }
}
