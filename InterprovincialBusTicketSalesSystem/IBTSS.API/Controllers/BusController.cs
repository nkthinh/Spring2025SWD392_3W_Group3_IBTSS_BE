using AutoMapper;
using IBTSS.Service.DTO.Request.Bus;
using IBTSS.Service.Services.BusService;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;

namespace IBTSS.API.Controllers
{
    [ApiController]
    [Route("api/buses")]
    public class BusController : ControllerBase
    {
        private readonly IBusService _busService;

        public BusController(IBusService busService)
        {
            _busService = busService;
        }

        [Authorize(Roles = "Admin")]
        [HttpPost]
        public async Task<IActionResult> CreateBus([FromBody] BusRequest request)
        {
            try
            {
                var result = await _busService.AddAsync(request);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = ex.Message });
            }
        }

        [Authorize(Roles = "Admin")]
        [HttpGet("{id}")]
        public async Task<IActionResult> GetBusById(string id)
        {
            try
            {
                var result = await _busService.GetByIdAsync(id);
                if (result == null)
                    return NotFound(new { message = "Not Found Bus" });

                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = ex.Message });
            }
        }

        [HttpGet]
        public async Task<IActionResult> GetFiltered([FromQuery] BusQueryParameters? query)
        {
            try
            {
                if (query == null || (string.IsNullOrEmpty(query.Keyword) && string.IsNullOrEmpty(query.SortBy) && query.Page == 0 && query.PageSize == 0))
                {
                    var all = await _busService.GetAllAsync();
                    return Ok(all);
                }
                if (query.Page == 0) query.Page = 1;
                if (query.PageSize == 0) query.PageSize = 10;
                var (data, totalCount) = await _busService.GetFilteredAsync(query);

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

        [Authorize(Roles = "Admin")]
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateBus(string id, [FromBody] BusUpdateRequest request)
        {
            try
            {
                var result = await _busService.UpdateAsync(id, request);
                if (result == null)
                    return NotFound(new { message = "Not Found Bus To Update" });

                return Ok(new
                {
                    message = "Deleted successfully",
                    data = result
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = ex.Message });
            }
        }
    }
}
