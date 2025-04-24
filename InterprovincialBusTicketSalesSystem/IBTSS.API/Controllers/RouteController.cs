using IBTSS.Service.DTO.Request.Route;
using IBTSS.Service.DTO.Response.Route;
using IBTSS.Service.Services.RouteService;
using Microsoft.AspNetCore.Mvc;

namespace IBTSS.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class RouteController : ControllerBase
    {
        private readonly IRouteService _routeService;

        public RouteController(IRouteService routeService)
        {
            _routeService = routeService;
        }

        [HttpGet]
        public async Task<ActionResult<List<RouteResponse>>> GetAll()
        {
            try
            {
                var result = await _routeService.GetAllAsync();
                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = ex.Message });
            }
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<RouteResponse>> GetById(string id)
        {
            try
            {
                var result = await _routeService.GetByIdAsync(id);
                if (result == null)
                    return NotFound(new { message = $"Route with ID '{id}' not found." });

                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = ex.Message });
            }
        }

        [HttpPost]
        public async Task<ActionResult<RouteResponse>> Add([FromBody] RouteRequest request)
        {
            try
            {
                var result = await _routeService.AddAsync(request);
                return CreatedAtAction(nameof(GetById), new { id = result.RouteId }, result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = ex.Message });
            }
        }

        [HttpPut("{id}")]
        public async Task<ActionResult<RouteResponse>> Update(string id, [FromBody] RouteRequest request)
        {
            try
            {
                var result = await _routeService.UpdateAsync(id, request);
                if (result == null)
                    return NotFound(new { message = $"Route with ID '{id}' not found." });

                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = ex.Message });
            }
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult> Delete(string id)
        {
            try
            {
                var result = await _routeService.DeleteAsync(id);
                if (!result)
                    return NotFound(new { message = $"Route with ID '{id}' not found." });

                return NoContent(); // 204
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = ex.Message });
            }
        }
    }
}
