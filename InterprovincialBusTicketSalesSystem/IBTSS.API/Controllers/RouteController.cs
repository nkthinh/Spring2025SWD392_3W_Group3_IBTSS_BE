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
            catch
            {
                return StatusCode(500, "Internal server error");
            }
        }
        [HttpGet("{id}")]
        public async Task<ActionResult<RouteResponse>> GetById(string id)
        {
            try
            {
                var result = await _routeService.GetByIdAsync(id);
                if (result == null)
                    return NotFound();
                return Ok(result);
            }
            catch
            {
                return StatusCode(500, "Internal server error");
            }
        }
        [HttpPost]
        public async Task<ActionResult<RouteResponse>> Add(RouteRequest request)
        {
            try
            {
                var result = await _routeService.AddAsync(request);
                return CreatedAtAction(nameof(GetById), new { id = result.RouteId }, result);
            }
            catch
            {
                return StatusCode(500, "Internal server error");
            }
        }
        [HttpPut]
        public async Task<ActionResult<RouteResponse>> Update(string id, RouteRequest request)
        {
            try
            {
                var result = await _routeService.UpdateAsync(id,request);
                if (result == null)
                    return NotFound();
                return Ok(result);
            }
            catch
            {
                return StatusCode(500, "Internal server error");
            }
        }
        [HttpDelete("{id}")]
        public async Task<ActionResult<bool>> Delete(string id)
        {
            try
            {
                var result = await _routeService.DeleteAsync(id);
                if (!result)
                    return NotFound();
                return Ok(result);
            }
            catch
            {
                return StatusCode(500, "Internal server error");
            }
        }
    }
}
