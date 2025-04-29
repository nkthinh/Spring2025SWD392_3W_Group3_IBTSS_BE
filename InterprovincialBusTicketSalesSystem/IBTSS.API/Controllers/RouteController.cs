using AutoMapper;
using IBTSS.Service.DTO.Request.Customer;
using IBTSS.Service.DTO.Request.Route;
using IBTSS.Service.DTO.Request.Trip;
using IBTSS.Service.DTO.Response.Customer;
using IBTSS.Service.DTO.Response.Route;
using IBTSS.Service.Services.CustomerService;
using IBTSS.Service.Services.RouteService;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace IBTSS.API.Controllers
{
    [Authorize(Roles = "Admin")]
    [ApiController]
    [Route("api/[controller]")]
    public class RouteController : ControllerBase
    {
        private readonly IRouteService _routeService;
        private readonly IMapper _mapper;

        public RouteController(IRouteService routeService, IMapper mapper)
        {
            _routeService = routeService;
            _mapper = mapper;
        }
        

        [HttpGet]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> GetFiltered([FromQuery] QueryParameters? query)
        {
            try
            {
                if (query == null || (string.IsNullOrEmpty(query.Keyword) && string.IsNullOrEmpty(query.SortBy) && query.Page == 0 && query.PageSize == 0))
                {
                    var all = await _routeService.GetAllAsync();
                    var mapped = _mapper.Map<IEnumerable<RouteResponse>>(all);
                    return Ok(mapped);
                }

                if (query.Page == 0) query.Page = 1;
                if (query.PageSize == 0) query.PageSize = 10;

                var (data, totalCount) = await _routeService.GetFilteredAsync(query);

                var pagination = new
                {
                    TotalCount = totalCount,
                    PageSize = query.PageSize,
                    CurrentPage = query.Page,
                    TotalPages = (int)Math.Ceiling((double)totalCount / query.PageSize)
                };

                return Ok(new { Data = data, Pagination = pagination });
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
                return Ok(new
                {
                    message = "Created successfully",
                    data = result
                });
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

                return Ok(new { message = "Deleted successfully" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = ex.Message });
            }
        }
    }
}
